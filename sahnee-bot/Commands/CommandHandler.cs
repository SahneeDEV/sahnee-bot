using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;
using sahnee_bot.Queue;

namespace sahnee_bot.commands
{
    public class CommandHandler
    {
        //Variables
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly Configuration.Configuration _configuration;
        private readonly Logger _logger = new Logger();
        private readonly IServiceProvider _service;
        private readonly IServiceProvider _messageService = new ServiceContainer();
        private readonly WarnAction _warnAction = new WarnAction();

        /// <summary>
        /// Creates a new CommandHandler
        /// </summary>
        /// <param name="client">the current client</param>
        /// <param name="commands">a new command service</param>
        /// <param name="configuration">the current configuration</param>
        /// <param name="service">the serviceprovider</param>
        public CommandHandler(DiscordSocketClient client, CommandService commands, Configuration.Configuration configuration, IServiceProvider service)
        {
            _client = client;
            _commands = commands;
            _configuration = configuration;
            _service = service;
        }

        /// <summary>
        /// Installs the CommandHandler
        /// </summary>
        /// <returns></returns>
        public async Task InstallCommandsAsync()
        {
            //Grabs the MessageReceived Event and executes it's own function
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _service);
            await _logger.Log("CommandHandler successfully initialized. Ready for Commands.", LogLevel.Info);
            _commands.CommandExecuted += OnCommandExecutedAsync;
        }

        /// <summary>
        /// Returns a user feeback on command execution
        /// </summary>
        /// <param name="command"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            //give the user feedback, what went wrong, if something went wrong
            if (!string.IsNullOrEmpty(result.ErrorReason))
            {
                await context.Channel.SendMessageAsync("🤔 " + result.ErrorReason);
                await _logger.Log($"On guild {context.Guild.Id} the following command error occured: {result.ErrorReason}",
                    LogLevel.Error);
            }
        }

        /// <summary>
        /// Handles all commands defined in Modules
        /// </summary>
        /// <param name="message">the current SocketMessage</param>
        /// <returns></returns>
        private async Task HandleCommandAsync(SocketMessage message)
        {
            //Check that the message is executed in the bot-commands channel
            if (message.Channel.Name != _configuration.General.CommandChannel)
            {
                return;
            }
            //Prevent from executing a system message or a bot message
            SocketUserMessage userMessage = message as SocketUserMessage;
            SocketCommandContext context = new SocketCommandContext(_client, userMessage);
            if (userMessage == null || userMessage.Author.IsBot) return;
            //check if the executed command is one that starts with the prefix
            int argPos = 0;
            //check for the prefix, also if there is a custom prefix
            WarningBotPrefixSchema guildPrefix = null;
            try
            {
                guildPrefix = StaticDatabase.WarningPrefixCollection().Query()
                    .Where(g => g.GuildId == context.Guild.Id)
                    .Single();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(System.InvalidOperationException))
                {
                    //ignore, because no custom prefix has been set
                }
                else
                {
                    await _logger.Log(e.Message, LogLevel.Error, "CommandHandler:HandleCommandAsync");
                }
            }

            if (!(userMessage.HasCharPrefix(
                guildPrefix != null ? guildPrefix.CustomPrefix : _configuration.General.CommandPrefix, ref argPos))) return;
            
            //check if a queue exists if not create a new one
            MultiThreadQueue queue = QueueFactory.GetQueueManager().CheckIfQueueForGuildExistsOrCreate(context.Guild.Id);
            //enqueue the message
            queue.Enqueue(new BasicQueueMessage() {context = context
                , argPos = argPos, serviceProvider = _service
                ,commands = _commands
            });
        }
    }
}
