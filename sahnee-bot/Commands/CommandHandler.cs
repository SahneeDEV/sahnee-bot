using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class CommandHandler
    {
        //Variables
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly Configuration _configuration;
        private readonly Logging _logging = new Logging();
        private readonly IServiceProvider _service;
        private readonly IServiceProvider _messageService = new ServiceContainer();
        private readonly WarnAction _warnAction = new WarnAction();

        /// <summary>
        /// Creates a new CommandHandler
        /// </summary>
        /// <param name="client">the current client</param>
        /// <param name="commands">a new command service</param>
        /// <param name="configuration">the current configuration</param>
        public CommandHandler(DiscordSocketClient client, CommandService commands, Configuration configuration, IServiceProvider service)
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
            await _logging.LogToConsoleBase("CommandHandler successfully initialized");
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
            if (userMessage == null || userMessage.Author.IsBot) return;
            //check if the executed command is one that starts with the prefix
            int argPos = 0;
            if (!(userMessage.HasCharPrefix(_configuration.General.CommandPrefix, ref argPos))) return;
            SocketCommandContext context = new SocketCommandContext(_client, userMessage);
            IResult result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _messageService);
            //punish the user if he made a mistake and messed up a command
            if (!result.IsSuccess)
            {
                try
                {
                    await StaticLock.AquireWarningAsync();
                    await this._warnAction.WarnAsync(message.Author as IGuildUser,
                        StaticConfiguration.GetConfiguration().WarningBot.PunishMessage, context.Guild, context.Channel, context.Message, StaticBot.GetBot().CurrentUser.Id);
                    await context.Channel.SendMessageAsync("Thats why it didn't work: " + result.ErrorReason);
                }
                finally
                {
                    StaticLock.UnlockCommandWarning();
                }
            }
        }
    }
}
