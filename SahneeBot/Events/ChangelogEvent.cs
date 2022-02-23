using Discord;
using Discord.WebSocket;
using SahneeBotController.Tasks;

namespace SahneeBot.Events;

/// <summary>
/// The changelog event is called whenever a guild is available and posts the newest changelog.
/// </summary>
[Event]
public class ChangelogEvent : EventBase<IGuild>
{
    private readonly DiscordSocketClient _bot;
    private readonly UpdateGuildChangelogTask _task;

    public ChangelogEvent(
        IServiceProvider serviceProvider,
        DiscordSocketClient bot,
        UpdateGuildChangelogTask task) : base(serviceProvider)
    {
        _bot = bot;
        _task = task;
    }

    public override void Register()
    {
        _bot.GuildAvailable += Handle;
    }

    public override Task Handle(IGuild arg) => HandleAsync(async ctx =>
    {
        await _task.Execute(ctx, new UpdateGuildChangelogTask.Args(arg.Id));
    }, new EventExecutionOptions
    {
        PlaceInQueue = arg.Id
    });
}