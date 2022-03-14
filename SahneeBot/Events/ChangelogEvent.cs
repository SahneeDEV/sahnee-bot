using Discord;
using SahneeBotController.Tasks;

namespace SahneeBot.Events;

/// <summary>
/// The changelog event is called whenever a guild is available and posts the newest changelog.
/// </summary>
[Event]
public class ChangelogEvent : EventBase<IGuild>
{
    private readonly Bot _bot;
    private readonly UpdateGuildChangelogTask _task;

    public ChangelogEvent(IServiceProvider serviceProvider
        , Bot bot
        , UpdateGuildChangelogTask task) : base(serviceProvider)
    {
        _bot = bot;
        _task = task;
    }

    public override void Register()
    {
        _bot.Impl(socket => socket.GuildAvailable += Handle
            , rest => 
                throw new InvalidOperationException("The changelog event only support the socket client."));
    }

    public override Task Handle(IGuild arg) => HandleAsync(async ctx => 
        await _task.Execute(ctx, new UpdateGuildChangelogTask.Args(arg.Id)), new EventExecutionOptions
    {
        PlaceInQueue = arg.Id
        , RelatedGuildId = arg.Id
        , Name = "changelog"
        , Debug = arg.Id.ToString()
    });
}