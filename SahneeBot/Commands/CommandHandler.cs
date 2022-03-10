using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SahneeBot.Commands;

/// <summary>
/// The command handler handles all slash command interaction.
/// </summary>
public class CommandHandler: ICommandHandler
{
    private readonly IServiceProvider _provider;
    private readonly Bot _bot;
    private readonly ILogger<CommandHandler> _logger;
    private InteractionService? _service;

    public CommandHandler(IServiceProvider provider
        , Bot bot
        , ILogger<CommandHandler> logger)
    {
        _provider = provider;
        _bot = bot;
        _logger = logger;
    }
    
    public async void Install()
    {
        _logger.LogInformation(EventIds.Startup, "Creating interaction handler...");

        // Hook up events
        await _bot.ImplAsync(
            async socket =>
            {
                // Creates the interaction service
                _service = new InteractionService(socket.Rest);
                // Dynamically add all command classes. Command classes must be public and inherit from InteractionModuleBase
                await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
                socket.GuildAvailable += GuildAvailable;
                socket.SlashCommandExecuted += SlashCommandExecuted;
            }
            , async rest => throw new InvalidOperationException("The command handler only support the socket client."));
    }

    /// <summary>
    /// Called whenever a slash command is executed.
    /// </summary>
    /// <param name="arg">The slash command.</param>
    private async Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        // Whenever a slash command is executed, create a context for the interaction and then run the command
        if (_service != null)
        {
            var ctx = new InteractionContext(_bot.Client, arg);
            await _service.ExecuteCommandAsync(ctx, _provider);
        }
    }

    /// <summary>
    /// Called whenever the bot joins a guild/receives information about a guild on startup.
    /// </summary>
    /// <param name="arg">The guild.</param>
    private async Task GuildAvailable(SocketGuild arg)
    {
        // Whenever the bot sees a new guild register the commands on it.
        if (_service != null)
        {
            _logger.LogDebug(EventIds.Startup, "Registering commands on guild {Name}", arg.Name);
            await _service.RegisterCommandsToGuildAsync(arg.Id);
        }
    }
}
