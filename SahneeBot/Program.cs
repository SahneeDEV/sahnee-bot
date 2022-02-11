using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

// Build to service provider
var serviceProvider = services.BuildServiceProvider(true);

// Define Discord Client
var discordConfig = new DiscordSocketConfig()
{
    GatewayIntents = GatewayIntents.All,
    AlwaysDownloadUsers = true
};
var discordSocketClient = new DiscordSocketClient(discordConfig);

// Block this task until the program is closed. <--- From the Discord.Net Guide
await Task.Delay(-1);
