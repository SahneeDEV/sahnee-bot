using Discord.WebSocket;
using SahneeBot.InteractionComponents;
using SahneeBotController;

namespace SahneeBot.Events;

[Event]
public class InteractionEvent : EventBase<SocketMessageComponent>
{
    private readonly Bot _bot;
    private readonly SelectMenuExecution _selectMenuExecution;

    public InteractionEvent(IServiceProvider serviceProvider
        , Bot bot
        , SelectMenuExecution selectMenuExecution) : base(serviceProvider)
    {
        _bot = bot;
        _selectMenuExecution = selectMenuExecution;
    }

    public override void Register()
    {
        _bot.Impl(socket => socket.SelectMenuExecuted += Handle
            , rest => 
                throw new InvalidOperationException("The interaction event only support the socket client."));
    }

    public override Task Handle(SocketMessageComponent arg) => HandleAsync(async ctx =>
    {
        await _selectMenuExecution.Execute(ctx, arg);
        return new Success<bool>(true);
    }, new EventExecutionOptions
    {
        // TODO: Guild
        RelatedUserId = arg.User.Id
        , Name = "interaction"
        , Debug = arg.Id.ToString()
    });
}
