using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SahneeBot;
using SahneeBot.Activity;
using SahneeBot.Commands;
using SahneeBot.Events;
using SahneeBot.Formatter;
using SahneeBot.Jobs;
using SahneeBot.Jobs.JobTasks;
using SahneeBot.Tasks;
using SahneeBotController.Tasks;
using SahneeBotModel;
using EventHandler = SahneeBot.Events.EventHandler;
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
        services.AddSingleton<IEventHandler, EventHandler>();
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
        services.AddTransient<ErrorDiscordFormatter>();
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
        services.AddTransient<WarningRolePrefixChangedDiscordFormatter>();
        services.AddTransient<TopUserWarnedDiscordFormatter>();
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
        services.AddTransient<ChangeWarningRoleNameTask>();
        services.AddTransient<GetLastChangelogOfGuildTask>();
        services.AddTransient<PostChangelogsToGuildTask, SahneeBotPostChangelogsToGuildTask>();
        services.AddTransient<UpdateGuildChangelogTask, SahneeBotUpdateGuildChangelogTask>();
        services.AddTransient<GetTopUserWarnedAmountTask>();
		// TASKS (BOT ONLY)
        services.AddTransient<SahneeBotChangeWarningRoleNameTask>();
        services.AddTransient<SahneeBotRoleLimitInformationTask>();
        services.AddTransient<SahneeBotReportErrorTask>();
        // JOBS
        services.AddTransient<CleanupWarningRolesJob>();
        // ACTIVITY
        services.AddTransient<BotActivity>();
        // DISCORD
        services.AddSingleton(provider =>
        {
            var discordConfig = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            };
            return new DiscordSocketClient(discordConfig);
        });
        services.AddSingleton(provider =>
        {
            var cfg = provider.GetRequiredService<IConfiguration>();
            var url = cfg["BotSettings:ErrorWebhookUrl"];
            return new DiscordWebhookClient(url);
        });
    })
    .Build();

var bot = host.Services.GetRequiredService<DiscordSocketClient>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var discordLogger = host.Services.GetRequiredService<DiscordLogger>();
var jobHandler = host.Services.GetRequiredService<JobHandler>();
var clearWarningRoles = host.Services.GetRequiredService<CleanupWarningRolesJob>();

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

//login the bot and start
var configuration = host.Services.GetRequiredService<IConfiguration>();
logger.LogInformation(EventIds.Startup, "Logging into discord API");
await bot.LoginAsync(TokenType.Bot, configuration["Discord:Token"]);
logger.LogInformation(EventIds.Startup, "Starting bot");
await bot.StartAsync();

var commandHandler = host.Services.GetRequiredService<ICommandHandler>();
commandHandler.Install();

var eventHandler = host.Services.GetRequiredService<IEventHandler>();
eventHandler.Install();

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