using Discord;
using Microsoft.Extensions.Logging;
using SahneeBotController;

namespace SahneeBot.Events;

/// <summary>
/// The changelog event is called whenever a guild is available and posts the newest changelog.
/// </summary>
[Event]
public class EnqueueGuildForChangelogEvent : EventBase<IGuild>
{
    private readonly ILogger<EnqueueGuildForChangelogEvent> _logger;
    private readonly Bot _bot;
    private readonly GuildChangelogQueue _queue;

    public EnqueueGuildForChangelogEvent(IServiceProvider serviceProvider
        , ILogger<EnqueueGuildForChangelogEvent> logger
        , Bot bot
        , GuildChangelogQueue queue) : base(serviceProvider)
    {
        _logger = logger;
        _bot = bot;
        _queue = queue;
    }

    public override void Register()
    {
        _bot.Impl(socket => socket.GuildAvailable += Handle
            , rest => 
                throw new InvalidOperationException("The changelog event only support the socket client."));
    }

    public override Task Handle(IGuild arg) => HandleAsync(ctx => 
    {
        _logger.LogDebug("Enqueue guild {Id} ({Name}) for the changelog event", arg.Id, arg.Name);
        _queue.Enqueue(arg.Id);
        return Task.FromResult<ISuccess>(new Success<bool>(true));
    }, new EventExecutionOptions 
    {
        PlaceInQueue = arg.Id
        , RelatedGuildId = arg.Id
        , Name = "prepare new changelogs"
        , Debug = arg.Id.ToString()
    });
}