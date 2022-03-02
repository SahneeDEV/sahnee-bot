using Discord.WebSocket;
using SahneeBot.InteractionComponents;

namespace SahneeBot.Events;

[Event]
public class InteractionEvent : EventBase<SocketMessageComponent>
{
    private readonly DiscordSocketClient _bot;
    private readonly SelectMenuExecution _selectMenuExecution;

    public InteractionEvent(IServiceProvider serviceProvider
        , DiscordSocketClient bot
        , SelectMenuExecution selectMenuExecution) : base(serviceProvider)
    {
        _bot = bot;
        _selectMenuExecution = selectMenuExecution;
    }

    public override void Register()
    {
        _bot.SelectMenuExecuted += Handle;
    }

    public override Task Handle(SocketMessageComponent arg) => HandleAsync(async ctx =>
    {
        await _selectMenuExecution.Execute(ctx, arg);
    });
}
