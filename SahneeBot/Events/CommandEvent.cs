using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBotController;

namespace SahneeBot.Events;

/// <summary>
/// The command handler handles all slash command interaction.
/// </summary>
[Event]
public class CommandEvent : EventBase<IGuild>
{
    private readonly Bot _bot;
    private readonly ILogger<CommandEvent> _logger;
    private readonly SahneeBotDiscordError _discordError;
    private InteractionService? _interactionService;

    public CommandEvent(IServiceProvider provider
        , Bot bot
        , ILogger<CommandEvent> logger, SahneeBotDiscordError discordError) : base(provider)
    {
        _bot = bot;
        _logger = logger;
        _discordError = discordError;
    }
    
    public override void Register()
    {
        // Hook up events
        _bot.ImplAsync(
            async socket =>
            {
                // Creates the interaction service
                _interactionService = new InteractionService(socket.Rest);
                // Dynamically add all command classes. Command classes must be public and inherit from InteractionModuleBase
                await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);
                socket.GuildAvailable += Handle;
                socket.SlashCommandExecuted += SlashCommandExecuted;
            }
            , rest => throw new InvalidOperationException("The command handler only support the socket client."));
    }

    public override Task Handle(IGuild arg) => HandleAsync(async ctx =>
    {
        if (_interactionService == null)
        {
            throw new ArgumentNullException(nameof(_interactionService));
        }
        _logger.LogDebug(EventIds.Startup, "Registering commands on guild {Name}", arg.Name);
        try
        {
            await _interactionService.RegisterCommandsToGuildAsync(arg.Id);
            return new Success<bool>(true);
        }
        catch (Exception exception)
        {
            var error = await _discordError.TryGetError<bool>(ctx, new SahneeBotDiscordError.ErrorOptions
            {
                Exception = exception
                , GuildId = arg.Id
            });
            if (error != null)
            {
                return error;
            }

            throw;
        }
    }, new EventExecutionOptions
    {
        PlaceInQueue = arg.Id
        , RelatedGuildId = arg.Id
        , Name = "slash-command registry"
        , Debug = arg.Id.ToString()
    });

    /// <summary>
    /// Called whenever a slash command is executed.
    /// </summary>
    /// <param name="arg">The slash command.</param>
    private async Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        // Whenever a slash command is executed, create a context for the interaction and then run the command
        if (_interactionService != null)
        {
            var ctx = new InteractionContext(_bot.Client, arg);
            await _interactionService.ExecuteCommandAsync(ctx, ServiceProvider);
        }
    }
}
