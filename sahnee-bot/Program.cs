using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.commands;
using sahnee_bot.Database;
using sahnee_bot.Jobs;
using sahnee_bot.Jobs.JobTasks;
using sahnee_bot.Util;


namespace sahnee_bot
{
    class Program
    {
        //Variables

        static void Main(string[] args)
        {
            Console.WriteLine("Loading Sahnee-Bot v0.9");
            new Program().MainTask().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Main Task. Logs in the bot and sets some other config things.
        /// </summary>
        /// <returns></returns>
        public async Task MainTask()
        {
            //Initialize the necessary classes
            DiscordSocketClient bot = new DiscordSocketClient();
            Logging logging = new Logging();
            LoadConfiguration loadConfiguration = new LoadConfiguration(true);
            Configuration configuration = loadConfiguration.GetConfiguration();
            CommandServiceConfig commandServiceConfig = new CommandServiceConfig();
            commandServiceConfig.DefaultRunMode = RunMode.Async;
            CommandHandler commandHandler = new CommandHandler(bot, new CommandService(commandServiceConfig), configuration, new ServiceContainer());
            StaticConfiguration.GetConfiguration();
            JobHandler jobHandler = new JobHandler();
            DiscordSocketConfig config = new DiscordSocketConfig {AlwaysDownloadUsers = true};
            StaticBot.SetBot(bot);

            //Add Eventhandlers
            bot.Log += logging.LogToConsole;
            await commandHandler.InstallCommandsAsync();
            
            //load the database
            StaticDatabase.LoadDatabase();

            //start the bot
            await bot.LoginAsync(TokenType.Bot, configuration.General.Token);
            if (bot.LoginState != LoginState.LoggedIn)
            {
                await logging.LogToConsoleBase("Could not login bot!");
                return;
            }
            await bot.StartAsync();
            await logging.LogToConsoleBase("Bot is logged in and started.");
            
            //Jobs
            //WarningRolesCleanup job
            jobHandler.RegisterJob(new JobTimeSpanRepeat(configuration.WarningBot.WarningRoleCleanup), async () =>
                {
                    await CleanupWarningRoles.CleanupWarningRolesRun(bot);
                });
            //Database cleanup job
            jobHandler.RegisterJob(new JobTimeSpanRepeat(configuration.General.DatabaseCleanup), async () =>
            {
                await ClearDatabaseLog.ClearDatabaseLogAsync();
            });

            // Block this task until the program is closed. <--- From the Discord.Net Guide
            await Task.Delay(-1);
        }
    }
}
