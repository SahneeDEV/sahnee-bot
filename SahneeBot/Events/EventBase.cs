using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Events;

public delegate Task EventDelegate(ITaskContext ctx);

public abstract class EventBase<TArg> : IEvent<TArg>
{
    protected IServiceProvider ServiceProvider { get; }
    private readonly ILogger<EventBase<TArg>> _logger;
    private readonly GuildQueue _queue;
    private readonly ErrorDiscordFormatter _errorFmt;

    protected EventBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        // We resolve all further classes manually instead of injection to keep the ctor simple for inheritance.
        _logger = serviceProvider.GetRequiredService<ILogger<EventBase<TArg>>>();
        _queue = serviceProvider.GetRequiredService<GuildQueue>();
        _errorFmt = serviceProvider.GetRequiredService<ErrorDiscordFormatter>();
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
        var scope = ServiceProvider.CreateScope();

        async Task ExecuteAsyncImpl()
        {
            _logger.LogDebug("Executing event {Event} on guild {Guild}", GetType().Name,
                opts.PlaceInQueue);
            try
            {
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
                // TODO: Report to admin?
                await _errorFmt.FormatAndSend(
                    new ErrorDiscordFormatter.Args("Event", GetType().Name, 
                        ToString() ?? "", opts.PlaceInQueue, null, exception),
                    DiscordFormat.Void);
            }
            scope.Dispose();
        }
        
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