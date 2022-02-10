using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBotController;
using SahneeBotModel;
using System;

namespace SahneeBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            new Program().MainTask().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Main Task. Initiates the Discord Bot
        /// </summary>
        public async Task MainTask()
        {
            //Variables
            IServiceProvider _serviceProvider;

            //Create a configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();

            //Register DI Services
            ServiceCollection? services = new ServiceCollection();

            //add services
            services.AddDbContext<SahneeBotModelContext>(options =>
            {
                options.UseNpgsql()
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            });
            services.AddSingleton<IConfiguration>(provider => configuration);


            //build to service provider
            _serviceProvider = services.BuildServiceProvider(true);
            
            

            //Define Discord Client
            DiscordSocketClient discordSocketClient;

            DiscordSocketConfig discordConfig = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            };

            discordSocketClient = new DiscordSocketClient(discordConfig);
            
            // Block this task until the program is closed. <--- From the Discord.Net Guide
            await Task.Delay(-1);
        }
    }
}