using Microsoft.Extensions.DependencyInjection;
using SahneeBot.Tasks;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Jobs;

public abstract class JobBase : IJob
{
    protected IServiceProvider ServiceProvider { get; }
    private readonly SahneeBotReportErrorTask _errorTask;
    private readonly SahneeBotTaskContextFactory _contextFactory;

    protected JobBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        _contextFactory = serviceProvider.GetRequiredService<SahneeBotTaskContextFactory>();
        _errorTask = serviceProvider.GetRequiredService<SahneeBotReportErrorTask>();
    }

    public delegate Task JobDelegate(ITaskContext ctx);
    
    public abstract Task Perform();
    public virtual IJobTimeSpan Time { get; protected init; } = JobTimeSpanNever.Instance;

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
        
        async Task ErrorReporter(ITaskContext ctx, Exception exception)
        {
            // Report error
            await _errorTask.Execute(ctx,
                new SahneeBotReportErrorTask.Args("Job", GetType().Name, ToString() ?? "", opts.PlaceInQueue, null,
                    exception));
        }
        
        await _contextFactory.ExecuteWithContextAsync(PerformAsyncImpl, new SahneeBotTaskContextFactory.ContextOptions
        {
            Type = "job",
            Name = GetType().Name,
            PlaceInQueue = opts.PlaceInQueue,
            ErrorReporter = ErrorReporter
        });
    }
}