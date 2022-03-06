using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SahneeBot.Activity;

/// <summary>
/// Updates the bot activity.
/// </summary>
public class BotActivity
{
    private readonly ILogger<BotActivity> _logger;
    private readonly DiscordSocketClient _bot;

    public BotActivity(ILogger<BotActivity> logger, DiscordSocketClient bot)
    {
        _logger = logger;
        _bot = bot;
    }

    /// <summary>
    /// Updates the bot activity
    /// </summary>
    public async Task UpdateBotActivity()
    {
        try
        {
            await _bot.SetActivityAsync(new ActivityWatchingGuildsAmount(_bot.Guilds.Count));
        }
        catch (Exception e)
        {
            _logger.LogCritical(EventIds.Discord, e, "Could not update the bot activity!");
        }
    }
    
}
