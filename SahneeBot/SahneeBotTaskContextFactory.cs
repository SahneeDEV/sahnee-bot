using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBot.Tasks.Error;
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
    private readonly SahneeBotReportExceptionTask _exceptionTask;
    private readonly GuildQueue _queue;

    /// <summary>
    /// The delegate to execute the context. Throw an exception to report an error or return a non successful success
    /// to rollback the transaction.
    /// </summary>
    public delegate Task<ISuccess> ContextDelegate(ITaskContext ctx);

    /// <summary>
    /// A delegate used to report an error in a context.
    /// </summary>
    public delegate Task ErrorReporterDelegate(ITaskContext ctx, ErrorReport report, ContextOptions ctxOpts);

    public record struct CrashInformation(string TicketId, Exception Exception);

    public record struct ErrorReport(CrashInformation? Crash, ISuccess? Error);

    public SahneeBotTaskContextFactory(IServiceProvider provider
        , ILogger<SahneeBotTaskContextFactory> logger
        , GuildQueue queue
        , SahneeBotReportExceptionTask exceptionTask)
    {
        _provider = provider;
        _logger = logger;
        _queue = queue;
        _exceptionTask = exceptionTask;
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
        /// A debug string for this context.
        /// </summary>
        public readonly string Debug { get; init; }
        /// <summary>
        /// To which guild ID is this context related?
        /// </summary>
        public readonly ulong? RelatedGuildId { get; init; }
        /// <summary>
        /// To which user ID is this context related?
        /// </summary>
        public readonly ulong? RelatedUserId { get; init; }
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
            Debug = "Context";
            PlaceInQueue = null;
            RelatedGuildId = null;
            RelatedUserId = null;
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
            using var ctx = new SahneeBotTaskContext(scope.ServiceProvider, scope, model, transaction);
            ISuccess error;
            var reportError = true;
            try
            {
                // Run context
                _logger.LogDebug(EventIds.Context
                    , "Executing {Name} {Type} on user/guild {User}/{Guild} (Queue: {Queue})"
                    , opts.Name, opts.Type, opts.RelatedUserId, opts.RelatedGuildId, opts.PlaceInQueue);
                error = await del(ctx);
            }
            catch (Exception exception)
            {
                // Report exception
                _logger.LogError(EventIds.Context
                    , exception, "{Error} in {Name} {Type} on guild {Guild}"
                    , exception.GetType().Name, opts.Name, opts.Type, opts.RelatedGuildId);
            
                var ticketId = await _exceptionTask.Execute(ctx, 
                    new SahneeBotReportExceptionTask.Args(opts.Type, opts.Name, opts.Debug
                        , opts.RelatedGuildId, opts.RelatedUserId, exception));
                var errorReport = new ErrorReport(new CrashInformation(ticketId, exception), null);
                if (opts.ErrorReporter != null)
                {
                    await opts.ErrorReporter(ctx, errorReport, opts);
                }
                reportError = false;
                
                // Context was not successful in case of error
                error = new Error<bool>(exception.Message);
            }
            // Commit transaction or rollback
            if (error.IsSuccess)
            {
                await transaction.CommitAsync();
            }
            else
            {
                try
                {
                    // Report error
                    _logger.LogWarning(EventIds.Context
                        ,"{Error} in {Name} {Type} on guild {Guild}"
                        , error.GetType().Name, opts.Name, opts.Type, opts.RelatedGuildId);
                    if (opts.ErrorReporter != null && reportError)
                    {
                        await opts.ErrorReporter(ctx, new ErrorReport(null, error), opts);
                    }
                }
                finally
                {
                    await transaction.RollbackAsync();
                }
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