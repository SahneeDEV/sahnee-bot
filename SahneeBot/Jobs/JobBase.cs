using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBot.Events;
using SahneeBot.Tasks;
using SahneeBotController;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Jobs;

public abstract class JobBase : IJob
{
    protected IServiceProvider ServiceProvider { get; }
    private readonly ILogger<JobBase> _logger;
    private readonly SahneeBotTaskContextFactory _contextFactory;

    protected JobBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<JobBase>>();
        _contextFactory = serviceProvider.GetRequiredService<SahneeBotTaskContextFactory>();
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
        async Task<ISuccess> PerformAsyncImpl(ITaskContext ctx)
        {
            await del(ctx);
            return new Success<bool>(true);
        }
        
        Task ErrorReporter(ITaskContext ctx, Exception exception)
        {
            _logger.LogError(EventIds.Jobs, exception, "Error in job {Job}", GetType().Name);
            return Task.CompletedTask;
        }

        await _contextFactory.ExecuteWithContextAsync(PerformAsyncImpl, new SahneeBotTaskContextFactory.ContextOptions
        {
            PlaceInQueue = opts.PlaceInQueue,
            ErrorReporter = ErrorReporter
        });
    }
}