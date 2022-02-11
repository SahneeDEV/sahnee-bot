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
    private readonly DiscordSocketClient _client;
    private readonly ILogger<CommandHandler> _logger;
    private InteractionService? _service;

    public CommandHandler(IServiceProvider provider, DiscordSocketClient client, ILogger<CommandHandler> logger)
    {
        _provider = provider;
        _client = client;
        _logger = logger;
    }
    
    /// <summary>
    /// Installs the command handler into discord.
    /// </summary>
    public async void Install()
    {
        _logger.LogInformation(EventIds.Startup, "Creating interaction handler...");
        // Creates the interaction service
        _service = new InteractionService(_client.Rest);
        // Dynamically add all command classes. Command classes must be public and inherit from InteractionModuleBase
        await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        // Hook up events
        _client.GuildAvailable += GuildAvailable;
        _client.SlashCommandExecuted += SlashCommandExecuted;
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
            var ctx = new InteractionContext(_client, arg);
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
            _logger.LogDebug(EventIds.Startup, "Registering commands on guild {name}", arg.Name);
            await _service.RegisterCommandsToGuildAsync(arg.Id);
        }
    }
}
