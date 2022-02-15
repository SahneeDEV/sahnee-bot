using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SahneeBot;
using SahneeBot.Commands;
using SahneeBot.Formatter;
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
        // FORMATTER
        services.AddSingleton<DefaultFormatArguments>();
        services.AddTransient<WarningDiscordFormatter>();
        services.AddTransient<CannotUnwarnDiscordFormatter>();
        services.AddTransient<HelpDiscordFormatter>();
        services.AddTransient<MissingPermissionDiscordFormatter>();
        services.AddTransient<RoleChangedDiscordFormatter>();
        // TASKS
        services.AddTransient<GiveWarningToUserTask>();
        services.AddTransient<GetUserGuildStateTask>();
        services.AddTransient<GetGuildStateTask>();
        services.AddTransient<AddRoleTask>();
        services.AddTransient<RemoveRoleTask>();
        services.AddTransient<SendWarningMessageToUserTask, SahneeBotSendWarningMessageToUserTask>();
        services.AddTransient<ModifyUserWarningGroupTask, SahneeBotModifyWarningGroupTask>();
        services.AddTransient<GetRolesOfUserTask, SahneeBotGetRolesOfUserTask>();
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

await host.RunAsync();