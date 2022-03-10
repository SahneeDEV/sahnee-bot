using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBotController;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot;

/// <summary>
/// A factory for creating a task context.
/// </summary>
public class SahneeBotTaskContextFactory
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<SahneeBotTaskContextFactory> _logger;
    private readonly GuildQueue _queue;

    /// <summary>
    /// The delegate to execute the context. Throw an exception to report an error or return a non successful success
    /// to rollback the transaction.
    /// </summary>
    public delegate Task<ISuccess> ContextDelegate(ITaskContext ctx);

    /// <summary>
    /// A delegate used to report an error in a context.
    /// </summary>
    public delegate Task ErrorReporterDelegate(ITaskContext ctx, Exception exception);

    public SahneeBotTaskContextFactory(IServiceProvider provider
        , ILogger<SahneeBotTaskContextFactory> logger
        , GuildQueue queue)
    {
        _provider = provider;
        _logger = logger;
        _queue = queue;
    }

    /// <summary>
    /// Context for executing a task.
    /// </summary>
    public struct ContextOptions
    {
        /// <summary>
        /// The type of context.
        /// </summary>
        public readonly string Type { get; init; }
        /// <summary>
        /// The name of context.
        /// </summary>
        public readonly string Name { get; init; }
        /// <summary>
        /// In which guild queue should the event be placed?
        /// </summary>
        public readonly ulong? PlaceInQueue { get; init; }
        /// <summary>
        /// Called if an error occurs.
        /// </summary>
        public readonly ErrorReporterDelegate? ErrorReporter { get; init; }

        public ContextOptions()
        {
            Type = "Context";
            Name = "Context";
            PlaceInQueue = null;
            ErrorReporter = null;
        }
    }
    
    /// <summary>
    /// Executes 
    /// </summary>
    /// <param name="del"></param>
    /// <param name="opts"></param>
    public async Task ExecuteWithContextAsync(ContextDelegate del, ContextOptions opts = default)
    {
        // Create scope for factory
        var scope = _provider.CreateScope();

        // Actual handler, called later
        async Task ExecuteWithContextAsyncImpl()
        {
            // Create context
            await using var model = scope.ServiceProvider.GetRequiredService<SahneeBotModelContext>();
            await using var transaction = await model.Database.BeginTransactionAsync();
            using var ctx = new SahneeBotTaskContext(opts.Type, scope.ServiceProvider, scope, model, transaction);
            ISuccess success;
            try
            {
                // Run context
                _logger.LogDebug(EventIds.Context, "Executing {Name} {Type} on guild {Guild}"
                    , opts.Name, ctx.Type, opts.PlaceInQueue);
                success = await del(ctx);
            }
            catch (Exception exception)
            {
                // Report error
                _logger.LogError(EventIds.Context
                    , exception, "Error in {Name} {Type} on guild {Guild}"
                    , opts.Name, ctx.Type, opts.PlaceInQueue);
                if (opts.ErrorReporter != null)
                {
                    await opts.ErrorReporter(ctx, exception);
                }
                // Context was not successful in case of error
                success = new Error<bool>(exception.Message);
            }
            // Commit transaction or rollback
            if (success.IsSuccess)
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }
            // Dispose provider scope
            scope.Dispose();
        }
        
        // Execute immediately or place in queue
        if (opts.PlaceInQueue != null)
        {
            var guildId = opts.PlaceInQueue.Value;
            _queue.Enqueue(guildId, ExecuteWithContextAsyncImpl);
        }
        else
        {
            await ExecuteWithContextAsyncImpl();
        }
    }
}