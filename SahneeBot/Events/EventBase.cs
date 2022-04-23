using Microsoft.Extensions.DependencyInjection;
using SahneeBot.Tasks.Error;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Events;

public delegate Task<ISuccess> EventDelegate(ITaskContext ctx);

public abstract class EventBase<TArg> : IEvent<TArg>
{
    protected IServiceProvider ServiceProvider { get; }
    private readonly SahneeBotTaskContextFactory _contextFactory;
    private readonly SahneeBotReportErrorToGuildAdministratorsTask _reportErrorToGuildAdministratorsTask;

    protected EventBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        // We resolve all further classes manually instead of injection to keep the ctor simple for inheritance.
        _contextFactory = serviceProvider.GetRequiredService<SahneeBotTaskContextFactory>();
        _reportErrorToGuildAdministratorsTask = serviceProvider.GetRequiredService<SahneeBotReportErrorToGuildAdministratorsTask>();
    }

    public record struct EventExecutionOptions
    {
        /// <summary>
        /// In which guild queue should the event be placed?
        /// </summary>
        public readonly ulong? PlaceInQueue { get; init; }
        /// <summary>
        /// To which guild ID is this event related?
        /// </summary>
        public readonly ulong? RelatedGuildId { get; init; }
        /// <summary>
        /// To which user ID is this event related?
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

    public abstract void Register();

    public Task Handle(object arg)
    {
        return Handle((TArg) arg);
    }

    public abstract Task Handle(TArg arg);

    protected async Task HandleAsync(EventDelegate del, EventExecutionOptions opts = default)
    {
        Task<ISuccess> HandleAsyncImpl(ITaskContext ctx) => del(ctx);
        
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

        await _contextFactory.ExecuteWithContextAsync(HandleAsyncImpl, new SahneeBotTaskContextFactory.ContextOptions
        {
            Type = "event"
            , Name = opts.Name ?? GetType().Name
            , Debug = opts.Debug ?? GetType().Name
            , PlaceInQueue = opts.PlaceInQueue
            , RelatedGuildId = opts.RelatedGuildId
            , RelatedUserId = opts.RelatedUserId
            , ErrorReporter = ErrorReporter
        });
    }
}