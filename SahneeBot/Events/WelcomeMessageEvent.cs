using Discord;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBot.Formatter.Error;
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
        if (channel == null)
        {
            return new Error<bool>("Please add a text channel to your sever and make sure the bot can access it.");
        }
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
                Exception = exception
                , GuildId = arg.Id
                , Hint = $"The bot does not have permissions to post its welcome message in the channel {channel.Mention}. If you do not want to grant it permissions to this channel please bind it to one of your other channels using `/config bind <channel>`."
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