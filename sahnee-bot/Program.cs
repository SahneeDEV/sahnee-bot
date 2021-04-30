using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.Activity;
using sahnee_bot.commands;
using sahnee_bot.Configuration;
using sahnee_bot.Database;
using sahnee_bot.Jobs;
using sahnee_bot.Jobs.JobTasks;
using sahnee_bot.Logging;
using sahnee_bot.OtherAPI.DiscordBotList;
using sahnee_bot.OtherAPI;
using sahnee_bot.Queue;
using sahnee_bot.Startup;
using sahnee_bot.Util;


namespace sahnee_bot
{
    class Program
    {
        //Variables

        static void Main(string[] args)
        {
            new Program().MainTask().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Main Task. Logs in the bot and sets some other config things.
        /// </summary>
        /// <returns></returns>
        public async Task MainTask()
        {
            //Initialize the necessary classes
            Logger logger = new Logger();
            DiscordSocketClient bot = new DiscordSocketClient();
            LoadConfiguration loadConfiguration = new LoadConfiguration(true);
            Configuration.Configuration configuration = loadConfiguration.GetConfiguration();
            Configuration.StaticConfiguration.GetConfiguration();
            
            CommandServiceConfig commandServiceConfig = new CommandServiceConfig();
            commandServiceConfig.DefaultRunMode = RunMode.Async;
            CommandHandler commandHandler = new CommandHandler(bot, new CommandService(commandServiceConfig), configuration, new ServiceContainer());
            JobHandler jobHandler = new JobHandler();
            DiscordSocketConfig config = new DiscordSocketConfig {AlwaysDownloadUsers = true};
            StaticBot.SetBot(bot);
            
            //Create a new QueueManager for the current instance
            QueueManager queueManager = QueueFactory.GetQueueManager();

            await logger.Log($"Loading Sahnee-Bot {StaticBot.GetVersion()}", LogLevel.Info);
            
            //load the database
            StaticDatabase.LoadDatabase();
            
            //Add Eventhandlers
            bot.Log += logger.Log;
            bot.JoinedGuild += BroadcastLatestChangeLog.AddNewlyJoinedGuild;
            bot.JoinedGuild += StartupTutorial.StartupTutorialAsync;
            bot.JoinedGuild += CreateBotCommandsChannel.CreateBotCommandsChannelAsync;
            bot.JoinedGuild += BotActivity.ChangeBotActivity;
            bot.JoinedGuild += SendApiFeedback.SendApiFeedbackAsync;
            bot.LeftGuild += BotActivity.ChangeBotActivity;
            bot.LeftGuild += SendApiFeedback.SendApiFeedbackAsync;
            await commandHandler.InstallCommandsAsync();
            
            //start the bot
            await bot.LoginAsync(TokenType.Bot, configuration.General.Token);
            if (bot.LoginState != LoginState.LoggedIn)
            {
                await logger.Log("Could not login bot!", LogLevel.Critical);
                return;
            }
            await bot.StartAsync();
            await logger.Log("Bot is logged in and started.", LogLevel.Info);
            
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
            
            //Changelog Announce procedure
            await BroadcastLatestChangeLog.BroadcastLatestChangeLogAsync(bot);
            //migrate roles if necessary
            await UpdateRoleSystem.UpdateRoleSystemAsync(bot.Guilds);
            //set the activity
            await BotActivity.ChangeBotActivity();

            //Add all available APIs
            SendApiFeedback.AddAvailableApi(new DiscordBotList());
            await SendApiFeedback.SendApiFeedbackAsync(null);

            // Block this task until the program is closed. <--- From the Discord.Net Guide
            await Task.Delay(-1);
        }
    }
}
