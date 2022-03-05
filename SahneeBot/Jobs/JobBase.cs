using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBot.Events;
using SahneeBot.Tasks;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Jobs;

public abstract class JobBase : IJob
{
    protected IServiceProvider ServiceProvider { get; }
    private readonly ILogger<JobBase> _logger;
    private readonly GuildQueue _queue;

    protected JobBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<JobBase>>();
        _queue = serviceProvider.GetRequiredService<GuildQueue>();
    }

    public delegate Task JobDelegate(ITaskContext ctx);
    
    public abstract Task Perform();

    public record struct JobExecutionOptions
    {
        /// <summary>
        /// In which guild queue should the event be placed?
        /// </summary>
        public readonly ulong? PlaceInQueue { get; init; }
    }

    protected async Task PerformAsync(JobDelegate del, JobExecutionOptions opts = default)
    {
        // Create scope for event
        var scope = ServiceProvider.CreateScope();

        // Actual handler, called later
        async Task ExecuteAsyncImpl()
        {
            _logger.LogDebug("Executing job {Job} on guild {Guild}", GetType().Name,
                opts.PlaceInQueue);
            // Create context
            await using var model = scope.ServiceProvider.GetRequiredService<SahneeBotModelContext>();
            await using var transaction = await model.Database.BeginTransactionAsync();
            using var ctx = new SahneeBotTaskContext(scope.ServiceProvider, scope, model, transaction);
            try
            {
                // Run command
                await del(ctx);
                // Commit transaction
                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                // Report error
                _logger.LogError(EventIds.Jobs, exception, "Error in job");
            }
            // Dispose provider scope
            scope.Dispose();
        }
        
        // Execute immediately or place in queue
        if (opts.PlaceInQueue != null)
        {
            var guildId = opts.PlaceInQueue.Value;
            _queue.Enqueue(guildId, ExecuteAsyncImpl);
        }
        else
        {
            await ExecuteAsyncImpl();
        }
    }
}