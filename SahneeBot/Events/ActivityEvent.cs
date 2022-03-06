using Discord;
using SahneeBot.Activity;

namespace SahneeBot.Events;

/// <summary>
/// Updates the bot activity when needed.
/// </summary>
[Event]
public class ActivityEvent : EventBase<IGuild?>
{
    private readonly Bot _bot;
    private readonly BotActivity _botActivity;

    public ActivityEvent(IServiceProvider serviceProvider
        , Bot bot
        , BotActivity botActivity) : base(serviceProvider)
    {
        _bot = bot;
        _botActivity = botActivity;
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
        await _botActivity.UpdateBotActivity();
    });
}