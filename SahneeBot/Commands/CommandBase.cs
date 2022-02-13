using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

public delegate Task CommandDelegate(ITaskContext ctx);

/// <summary>
/// Base class for commands.
/// </summary>
public abstract class CommandBase: InteractionModuleBase<IInteractionContext>
{
    /// <summary>
    /// The service provider of the command.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;
    
    private readonly ILogger<CommandBase> _logger;
    private readonly GuildQueue _queue;

    /// <summary>
    /// Creates a new command base class.
    /// </summary>
    /// <param name="serviceProvider">The provider to use for DI.</param>
    public CommandBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        // We resolve all further classes manually instead of injection to keep the ctor simple for inheritance.
        _logger = serviceProvider.GetRequiredService<ILogger<CommandBase>>();
        _queue = serviceProvider.GetRequiredService<GuildQueue>();
    }
    
    /// <summary>
    /// Options for executing a command.
    /// </summary>
    public struct CommandExecutionOptions
    {
        /// <summary>
        /// Should the command be placed in the guild queue?
        /// </summary>
        public readonly bool PlaceInQueue;
        /// <summary>
        /// Is the defer response ephemeral?
        /// </summary>
        public readonly bool DeferEphemeral;
        /// <summary>
        /// The request options of the defer response
        /// </summary>
        public readonly RequestOptions? DeferRequest;

        /// <summary>
        /// Creates command execution options.
        /// </summary>
        /// <param name="placeInQueue">Place the command in the guild queue?</param>
        /// <param name="deferEphemeral">Set ephemeral in the defer response?</param>
        /// <param name="deferRequest">The request options of the defer response</param>
        public CommandExecutionOptions(bool placeInQueue, bool deferEphemeral, RequestOptions? deferRequest = null)
        {
            PlaceInQueue = placeInQueue;
            DeferEphemeral = deferEphemeral;
            DeferRequest = deferRequest;
        }

        /// <summary>
        /// Creates command execution options.
        /// </summary>
        /// <param name="placeInQueue">Place the command in the guild queue?</param>
        public CommandExecutionOptions(bool placeInQueue)
        {
            PlaceInQueue = placeInQueue;
            DeferEphemeral = false;
            DeferRequest = null;
        }
    }
    
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="del">The command delegate.</param>
    /// <param name="opts">Options to customize command execution.</param>
    protected async Task ExecuteAsync(CommandDelegate del, CommandExecutionOptions opts = default)
    {
        var scope = ServiceProvider.CreateScope();
        await DeferAsync(opts.DeferEphemeral, opts.DeferRequest);
        
        async Task ExecuteAsyncImpl()
        {
            using (scope)
            {
                try
                {
                    // Create context
                    await using var model = scope.ServiceProvider.GetRequiredService<SahneeBotModelContext>();
                    await using var transaction = await model.Database.BeginTransactionAsync();
                    using var ctx = new SahneeBotTaskContext(scope.ServiceProvider, scope, model, transaction);
                    // Run command
                    await del(ctx);
                    // Commit transaction
                    await transaction.CommitAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogError(EventIds.Command, exception, "Error in {Command} command", Context.Interaction);
                }
            }
        }

        if (opts.PlaceInQueue)
        {
            var guildId = Context.Guild?.Id;
            if (!guildId.HasValue)
            {
                throw new InvalidOperationException("Cannot place global commands in a guild queue");
            }
            _queue.Enqueue(guildId.Value, ExecuteAsyncImpl);
        }
        else
        {
            await ExecuteAsyncImpl();
        }
    }
}
