using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;

namespace SahneeBot.Events;

/// <summary>
/// Sends the welcome message to the guild owner.
/// </summary>
[Event]
public class WelcomeMessageEvent : EventBase<IGuild>
{
    private readonly Bot _bot;
    private readonly ILogger<WelcomeMessageEvent> _logger;
    private readonly WelcomeOnNewGuildJoinDiscordFormatter _welcomeOnNewGuildJoinDiscordFormatter;
    private readonly PrivateMessageToGuildOwnerFormatter _privateMessageToGuildOwnerFormatter;

    public WelcomeMessageEvent(IServiceProvider serviceProvider
        , Bot bot
        , ILogger<WelcomeMessageEvent> logger
        , WelcomeOnNewGuildJoinDiscordFormatter welcomeOnNewGuildJoinDiscordFormatter
        , PrivateMessageToGuildOwnerFormatter privateMessageToGuildOwnerFormatter) : base(serviceProvider)
    {
        _bot = bot;
        _logger = logger;
        _welcomeOnNewGuildJoinDiscordFormatter = welcomeOnNewGuildJoinDiscordFormatter;
        _privateMessageToGuildOwnerFormatter = privateMessageToGuildOwnerFormatter;
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
        if (channel == null)
        {
            var guildOwner = await arg.GetOwnerAsync();
            if (guildOwner == null)
            {
                _logger.LogCritical(EventIds.Discord, "Could not even find a guild owner to notify" +
                                                      " that I cannot send a message!");
                throw new Exception($"Failed getting Guild Owner in guild: {arg.Id}");
            }

            await _privateMessageToGuildOwnerFormatter.FormatAndSend(
                new PrivateMessageToGuildOwnerFormatter.Args(arg.Name, "Cannot send initial message!"
                    , "An error with the bot permissions"
                    , "You invited the bot with not enough permissions. Please re-invite the" +
                      " bot with the suggested permissions. If you are worried about our data processing," +
                      "you can refer to our [privacy policy](https://sahnee.dev/en/sahnee-bot-privacy-policy/)")
                , guildOwner.SendMessageAsync);
            return;
        }

        await _welcomeOnNewGuildJoinDiscordFormatter.FormatAndSend(
            new WelcomeOnNewGuildJoinDiscordFormatter.Args(arg.Name)
            , channel.SendMessageAsync);
    });
}