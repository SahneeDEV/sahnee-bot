using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBot.Tasks;
using SahneeBotController;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Events;

public delegate Task EventDelegate(ITaskContext ctx);

public abstract class EventBase<TArg> : IEvent<TArg>
{
    protected IServiceProvider ServiceProvider { get; }
    private readonly SahneeBotReportErrorTask _errorTask;
    private readonly SahneeBotTaskContextFactory _contextFactory;

    protected EventBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        // We resolve all further classes manually instead of injection to keep the ctor simple for inheritance.
        _errorTask = serviceProvider.GetRequiredService<SahneeBotReportErrorTask>();
        _contextFactory = serviceProvider.GetRequiredService<SahneeBotTaskContextFactory>();
    }

    public record struct EventExecutionOptions
    {
        /// <summary>
        /// In which guild queue should the event be placed?
        /// </summary>
        public readonly ulong? PlaceInQueue { get; init; }
    }

    public abstract void Register();

    public Task Handle(object arg)
    {
        return Handle((TArg) arg);
    }

    public abstract Task Handle(TArg arg);

    protected async Task HandleAsync(EventDelegate del, EventExecutionOptions opts = default)
    {
        async Task<ISuccess> HandleAsyncImpl(ITaskContext ctx)
        {
            await del(ctx);
            return new Success<bool>(true);
        }
        
        async Task ErrorReporter(ITaskContext ctx, Exception exception)
        {
            // Report error
            await _errorTask.Execute(ctx,
                new SahneeBotReportErrorTask.Args("Event", GetType().Name, ToString() ?? "", opts.PlaceInQueue, null,
                    exception));
        }

        await _contextFactory.ExecuteWithContextAsync(HandleAsyncImpl, new SahneeBotTaskContextFactory.ContextOptions
        {
            PlaceInQueue = opts.PlaceInQueue,
            ErrorReporter = ErrorReporter
        });
    }
}