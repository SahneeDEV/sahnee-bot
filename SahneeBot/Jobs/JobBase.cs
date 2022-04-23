using Microsoft.Extensions.DependencyInjection;
using SahneeBot.Tasks.Error;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Jobs;

/// <summary>
/// Base class for implementing custom jobs.
/// </summary>
public abstract class JobBase : IJob
{
    /// <summary>
    /// The service provider.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }
    private readonly SahneeBotTaskContextFactory _contextFactory;
    private readonly SahneeBotReportErrorToGuildAdministratorsTask _reportErrorToGuildAdministratorsTask;

    protected JobBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        _contextFactory = serviceProvider.GetRequiredService<SahneeBotTaskContextFactory>();
        _reportErrorToGuildAdministratorsTask = serviceProvider.GetRequiredService<SahneeBotReportErrorToGuildAdministratorsTask>();
    }

    public delegate Task<ISuccess> JobDelegate(ITaskContext ctx);
    
    public abstract Task Perform();
    public virtual IJobTimeSpan Time { get; protected init; } = JobTimeSpanNever.Instance;

    public record struct JobExecutionOptions
    {
        /// <summary>
        /// In which guild queue should the job be placed?
        /// </summary>
        public readonly ulong? PlaceInQueue { get; init; }
        /// <summary>
        /// To which guild ID is this job related?
        /// </summary>
        public readonly ulong? RelatedGuildId { get; init; }
        /// <summary>
        /// To which user ID is this job related?
        /// </summary>
        public readonly ulong? RelatedUserId { get; init; }
        /// <summary>
        /// A name of the event.
        /// </summary>
        public readonly string? Name { get; init; }
        /// <summary>
        /// A event debug information.
        /// </summary>
        public readonly string? Debug { get; init; }
    }

    /// <summary>
    /// Performs this job.
    /// </summary>
    /// <param name="del">The job delegate.</param>
    /// <param name="opts">Options for executing the job.</param>
    protected async Task PerformAsync(JobDelegate del, JobExecutionOptions opts = default)
    {
        Task<ISuccess> PerformAsyncImpl(ITaskContext ctx) => del(ctx);
        
        async Task ErrorReporter(ITaskContext ctx, SahneeBotTaskContextFactory.ErrorReport report
            , SahneeBotTaskContextFactory.ContextOptions ctxOpts)
        {
            if (report.Error != null)
            {
                var args = SahneeBotReportErrorToGuildAdministratorsTask.Args.FromErrorReport(report, ctxOpts);
                if (args.HasValue)
                {
                    await _reportErrorToGuildAdministratorsTask.Execute(ctx, args.Value);
                }
            }
        }
        
        await _contextFactory.ExecuteWithContextAsync(PerformAsyncImpl, new SahneeBotTaskContextFactory.ContextOptions
        {
            Type = "background job"
            , Name = opts.Name ?? GetType().Name
            , Debug = opts.Debug ?? GetType().Name
            , PlaceInQueue = opts.PlaceInQueue
            , RelatedGuildId = opts.RelatedGuildId
            , RelatedUserId = opts.RelatedUserId
            , ErrorReporter = ErrorReporter
        });
    }
}