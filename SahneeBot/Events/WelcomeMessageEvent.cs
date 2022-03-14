using Discord;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController;

namespace SahneeBot.Events;

/// <summary>
/// Sends the welcome message to the guild owner.
/// </summary>
[Event]
public class WelcomeMessageEvent : EventBase<IGuild>
{
    private readonly Bot _bot;
    private readonly WelcomeOnNewGuildJoinDiscordFormatter _welcomeOnNewGuildJoinDiscordFormatter;
    private readonly SahneeBotDiscordError _discordError;

    public WelcomeMessageEvent(IServiceProvider serviceProvider
        , Bot bot
        , WelcomeOnNewGuildJoinDiscordFormatter welcomeOnNewGuildJoinDiscordFormatter
        , SahneeBotDiscordError discordError) : base(serviceProvider)
    {
        _bot = bot;
        _welcomeOnNewGuildJoinDiscordFormatter = welcomeOnNewGuildJoinDiscordFormatter;
        _discordError = discordError;
    }

    public override void Register()
    {
        _bot.Impl(socket => socket.JoinedGuild += Handle
            , rest => 
                throw new InvalidOperationException("The welcome message event only support the socket client."));
    }

    public override Task Handle(IGuild arg) => HandleAsync(async ctx =>
    {
        //get the default message channel
        var channel = await arg.GetDefaultChannelAsync();
        try
        {
            await _welcomeOnNewGuildJoinDiscordFormatter.FormatAndSend(
                new WelcomeOnNewGuildJoinDiscordFormatter.Args(arg.Name), channel.SendMessageAsync);
            return new Success<bool>(true);
        }
        catch (Exception exception)
        {
            var error = await _discordError.TryGetError<bool>(ctx, new SahneeBotDiscordError.ErrorOptions
            {
                Exception = exception, GuildId = arg.Id
            });
            if (error != null)
            {
                return error;
            }

            throw;
        }
    }, new EventExecutionOptions
    {
        RelatedGuildId = arg.Id
        , Name = "welcome"
        , Debug = arg.Id.ToString()
    });
}