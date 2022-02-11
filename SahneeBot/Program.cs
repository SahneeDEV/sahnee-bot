using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBot;
using SahneeBot.Commands;
using SahneeBotController.Tasks;
using SahneeBotModel;

// Create a configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

// Register DI Services
var services = new ServiceCollection();

// Add services
services.AddLogging(options =>
{
    options
        .SetMinimumLevel(LogLevel.Debug)
        .AddConsole();
});
services.AddDbContext<SahneeBotModelContext>(options =>
{
    options
        .UseNpgsql()
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
});
services.AddSingleton<IConfiguration>(provider => configuration);
services.AddSingleton(provider => new IdGenerator(1));
services.AddSingleton<ICommandHandler, CommandHandler>();
services.AddTransient<GiveWarningToUserTask>();

// Define Discord Client
//var intents = GatewayIntents.Guilds;
var discordConfig = new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.All,
    AlwaysDownloadUsers = true
};
var discordSocketClient  = new DiscordSocketClient(discordConfig);
services.AddSingleton(provider => discordSocketClient);

// Build to service provider
var serviceProvider = services.BuildServiceProvider(true);
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
discordSocketClient.Log += logMessage =>
{
    LogLevel level = LogLevel.None;
    switch (logMessage.Severity)
    {
        case LogSeverity.Critical:
        {
            level = LogLevel.Critical;
            break;
        }
        case LogSeverity.Debug:
        {
            level = LogLevel.Debug;
            break;
        }
        case LogSeverity.Error:
        {
            level = LogLevel.Error;
            break;
        }
        case LogSeverity.Info:
        {
            level = LogLevel.Information;
            break;
        }
        case LogSeverity.Verbose:
        {
            level = LogLevel.Trace;
            break;
        }
        case LogSeverity.Warning:
        {
            level = LogLevel.Warning;
            break;
        }
    }

    var message = string.IsNullOrEmpty(logMessage.Message) ? logMessage.Exception?.Message : logMessage.Message;
    if (logMessage.Exception != null)
    {
        logger.Log(level, EventIds.Discord, logMessage.Exception, message + " (" + logMessage.Source +  ")");
    }
    else
    {
        logger.Log(level, EventIds.Discord, message + " (" + logMessage.Source +  ")");
    }
    return Task.CompletedTask;
};

//Add EventHandlers to the bot
//bot.Log += 
//bot.JoinedGuild += 
//bot.LeftGuild += 
//bot.SlashCommandExecuted += 

//login the bot and start
logger.LogInformation(EventIds.Startup, "Logging into discord API...");
await discordSocketClient.LoginAsync(TokenType.Bot, configuration["Discord:Token"]);
logger.LogInformation(EventIds.Startup, "Starting bot...");
await discordSocketClient.StartAsync();

var commandHandler = serviceProvider.GetRequiredService<ICommandHandler>();
commandHandler.Install();

// Block this task until the program is closed. <--- From the Discord.Net Guide
await Task.Delay(-1);
