using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SahneeBot;
using SahneeBot.Activity;
using SahneeBot.Commands;
using SahneeBot.Formatter;
using SahneeBot.Jobs;
using SahneeBot.Jobs.JobTasks;
using SahneeBot.Tasks;
using SahneeBotController.Tasks;
using SahneeBotModel;
using EventIds = SahneeBot.EventIds;

static IHostBuilder CreateHostBuilder(string[] args)
    => Host
        .CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            // ID GENERATOR
            services.AddSingleton(provider =>
            {
                var cfg = provider.GetRequiredService<IConfiguration>();
                var machineId = long.Parse(cfg["MachineId"]);
                return new IdGenerator(machineId);
            });
            // MODEL
            services.AddDbContext<SahneeBotModelContext>(options =>
            {
                options
                    .UseNpgsql()
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            });
        });

var host = CreateHostBuilder(args)
    .ConfigureServices(services =>
    {
        // SYSTEM
        services.AddSingleton<DiscordLogger>();
        services.AddSingleton<GuildQueue>();
        services.AddSingleton<ICommandHandler, CommandHandler>();
        services.AddSingleton<Changelog>();
        services.AddSingleton<JobHandler>();
        // FORMATTER
        services.AddSingleton<DefaultFormatArguments>();
        services.AddTransient<WarningDiscordFormatter>();
        services.AddTransient<CannotUnwarnDiscordFormatter>();
        services.AddTransient<HelpDiscordFormatter>();
        services.AddTransient<MissingPermissionDiscordFormatter>();
        services.AddTransient<RoleDiscordFormatter>();
        services.AddTransient<RoleChangedDiscordFormatter>();
        services.AddTransient<NoWarningFoundDiscordFormatter>();
        services.AddTransient<CommandErrorDiscordFormatter>();
        services.AddTransient<RoleColorChangeDiscordFormatter>();
        services.AddTransient<GeneralErrorDiscordFormatter>();
        services.AddTransient<BoundChannelDiscordFormatter>();
        services.AddTransient<NotBoundChannelDiscordFormatter>();
        services.AddTransient<MessageOptOutHintDiscordFormatter>();
        services.AddTransient<MessageOptOutDiscordFormatter>();
        services.AddTransient<ChangelogVersionDiscordFormatter>();
        services.AddTransient<NoChangelogFoundDiscordFormatter>();
        services.AddTransient<WarningRoleCleanupDiscordFormatter>();
        services.AddTransient<WarningRoleSetDiscordFormatter>();
        services.AddTransient<WelcomeOnNewGuildJoinDiscordFormatter>();
        services.AddTransient<PrivateMessageToGuildOwnerFormatter>();
        services.AddTransient<InformRoleLimitDiscordFormatter>();
        // TASKS
        services.AddTransient<GiveWarningToUserTask>();
        services.AddTransient<GetUserGuildStateTask>();
        services.AddTransient<GetGuildStateTask>();
        services.AddTransient<AddRoleTask>();
        services.AddTransient<RemoveRoleTask>();
        services.AddTransient<ChangeRoleColorTask, SahneeBotGuildChangeRoleColorTask>();
        services.AddTransient<SendWarningMessageToUserTask, SahneeBotSendWarningMessageToUserTask>();
        services.AddTransient<ModifyUserWarningGroupTask, SahneeBotModifyWarningGroupTask>();
        services.AddTransient<GetRolesOfUserTask, SahneeBotGetRolesOfUserTask>();
        services.AddTransient<GetRolesOfGuildTask>();
        services.AddTransient<GetRandomWarningsTask>();
        services.AddTransient<GetAllWarningsCreatedFromToTask>();
        services.AddTransient<GetLastWarningsTask>();
        services.AddTransient<ChangeBoundChannelTask>();
        services.AddTransient<GetBoundChannelTask>();
        services.AddTransient<SendMessageOptOutHintToUserTask, SahneeBotSendMessageOptOutHintToUserTask>();
        services.AddTransient<MessageOptOutTask>();
        services.AddTransient<GetMessageOptOutTask>();
        services.AddTransient<SetGuildRoleSetTask>();
        services.AddTransient<GetLastChangelogOfGuildTask>();
        services.AddTransient<SahneeBotJoinedGuildTask>();
        services.AddTransient<SahneeBotRoleLimitInformationTask>();
        services.AddTransient<SahneeBotLeftGuildTask>();
        // JOBS
        services.AddTransient<CleanupWarningRolesJobTask>();
        // ACTIVITY
        services.AddTransient<BotActivity>();
        // DISCORD
        var discordConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            AlwaysDownloadUsers = true
        };
        var discordSocketClient  = new DiscordSocketClient(discordConfig);
        services.AddSingleton(provider => discordSocketClient);
    })
    .Build();

var bot = host.Services.GetRequiredService<DiscordSocketClient>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var discordLogger = host.Services.GetRequiredService<DiscordLogger>();
var jobHandler = host.Services.GetRequiredService<JobHandler>();
var clearWarningRoles = host.Services.GetRequiredService<CleanupWarningRolesJobTask>();
var joinedTask = host.Services.GetRequiredService<SahneeBotJoinedGuildTask>();
var leftTask = host.Services.GetRequiredService<SahneeBotLeftGuildTask>();
var botActivity = host.Services.GetRequiredService<BotActivity>();

using (var scope = host.Services.CreateScope())
{
    logger.LogInformation(EventIds.Migration, "Running database migrations");
    try
    {
        var ctx = scope.ServiceProvider.GetRequiredService<SahneeBotModelContext>();
        await ctx.Database.MigrateAsync();
    }
    catch (Exception error)
    {
        logger.LogError(EventIds.Migration, error, "Failed to run migrations");
        return;
    }
}

bot.Log += discordLogger.Log;
bot.JoinedGuild += joinedTask.JoinedGuildTask;
bot.LeftGuild += leftTask.LeftGuildTask;
bot.Ready += botActivity.UpdateBotActivity;

//login the bot and start
var configuration = host.Services.GetRequiredService<IConfiguration>();
logger.LogInformation(EventIds.Startup, "Logging into discord API");
await bot.LoginAsync(TokenType.Bot, configuration["Discord:Token"]);
logger.LogInformation(EventIds.Startup, "Starting bot");
await bot.StartAsync();

var commandHandler = host.Services.GetRequiredService<ICommandHandler>();
commandHandler.Install();

//register the jobs
var guid = jobHandler.RegisterJob(new JobHandler.Args(new JobTimeSpanRepeat(
        TimeSpan.FromMinutes(int.Parse(configuration["BotSettings:Jobs:CleanupWarningRoles"]))),
    async () =>
    {
        await clearWarningRoles.CleanupWarningRolesRun(host.Services);
    }));
logger.LogDebug(EventIds.Jobs, "Registered Job for cleaning warning roles" +
                               " with guid: {guid}", guid);

await host.RunAsync();