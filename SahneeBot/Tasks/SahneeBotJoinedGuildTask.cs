using Discord;
using Microsoft.Extensions.Logging;
using SahneeBot.Activity;
using SahneeBot.Formatter;

namespace SahneeBot.Tasks;

public class SahneeBotJoinedGuildTask
{
    private readonly ILogger<SahneeBotJoinedGuildTask> _logger;
    private readonly WelcomeOnNewGuildJoinDiscordFormatter _welcomeOnNewGuildJoinDiscordFormatter;
    private readonly PrivateMessageToGuildOwnerFormatter _privateMessageToGuildOwnerFormatter;
    private readonly BotActivity _botActivity;

    public SahneeBotJoinedGuildTask(ILogger<SahneeBotJoinedGuildTask> logger
        , WelcomeOnNewGuildJoinDiscordFormatter welcomeOnNewGuildJoinDiscordFormatter
        , PrivateMessageToGuildOwnerFormatter privateMessageToGuildOwnerFormatter
        , BotActivity botActivity)
    {
        _logger = logger;
        _welcomeOnNewGuildJoinDiscordFormatter = welcomeOnNewGuildJoinDiscordFormatter;
        _privateMessageToGuildOwnerFormatter = privateMessageToGuildOwnerFormatter;
        _botActivity = botActivity;
    }

    public async Task JoinedGuildTask(IGuild guild)
    {
        //get the default message channel
        var channel = await guild.GetDefaultChannelAsync();
        if (channel == null)
        {
            var guildOwner = await guild.GetOwnerAsync();
            if (guildOwner == null)
            {
                _logger.LogCritical(EventIds.Discord, "Could not even find a guild owner to notify" +
                                                      " that I cannot send a message!");
                throw new Exception($"Failed getting Guild Owner in guild: {guild.Id}");
            }

            await _privateMessageToGuildOwnerFormatter.FormatAndSend(
                new PrivateMessageToGuildOwnerFormatter.Args(guild.Name, "Cannot send initial message!"
                    , "An error with the bot permissions"
                    , "You invited the bot with not enough permissions. Please re-invite the" +
                      " bot with the suggested permissions. If you are worried about our data processing," +
                      "you can refer to our [privacy policy](https://sahnee.dev/en/sahnee-bot-privacy-policy/)")
                , guildOwner.SendMessageAsync);
            await _botActivity.UpdateBotActivity();
            return;
        }

        await _welcomeOnNewGuildJoinDiscordFormatter.FormatAndSend(
            new WelcomeOnNewGuildJoinDiscordFormatter.Args(guild.Name)
            , channel.SendMessageAsync);
        await _botActivity.UpdateBotActivity();
    }
}
