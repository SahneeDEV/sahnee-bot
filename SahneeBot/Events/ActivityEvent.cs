using Discord;
using Discord.WebSocket;
using SahneeBot.Activity;

namespace SahneeBot.Events;

/// <summary>
/// Updates the bot activity when needed.
/// </summary>
[Event]
public class ActivityEvent : EventBase<IGuild?>
{
    private readonly DiscordSocketClient _bot;
    private readonly BotActivity _botActivity;

    public ActivityEvent(
        IServiceProvider serviceProvider,
        DiscordSocketClient bot,
        BotActivity botActivity
        ) : base(serviceProvider)
    {
        _bot = bot;
        _botActivity = botActivity;
    }

    public override void Register()
    {
        _bot.JoinedGuild += Handle;
        _bot.LeftGuild += Handle;
        _bot.Ready += () => Handle(null);
    }

    public override Task Handle(IGuild? arg) => HandleAsync(async ctx =>
    {
        await _botActivity.UpdateBotActivity();
    });
}