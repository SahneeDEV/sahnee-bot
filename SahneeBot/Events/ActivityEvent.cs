using Discord;
using SahneeBot.Tasks;

namespace SahneeBot.Events;

/// <summary>
/// Updates the bot activity when needed.
/// </summary>
[Event]
public class ActivityEvent : EventBase<IGuild?>
{
    private readonly Bot _bot;
    private readonly SahneeBotActivityTask _sahneeBotActivityTask;

    public ActivityEvent(IServiceProvider serviceProvider
        , Bot bot
        , SahneeBotActivityTask sahneeBotActivityTask) : base(serviceProvider)
    {
        _bot = bot;
        _sahneeBotActivityTask = sahneeBotActivityTask;
    }

    public override void Register()
    {
        _bot.Impl(socket =>
            {
                socket.JoinedGuild += Handle;
                socket.LeftGuild += Handle;
                socket.Ready += () => Handle(null);
            }
            , rest =>
                throw new InvalidOperationException("The activity event only support the socket client."));
    }

    public override Task Handle(IGuild? arg) => HandleAsync(async ctx =>
    {
        await _sahneeBotActivityTask.Execute(ctx, new SahneeBotActivityTask.Args());
        
    });
}