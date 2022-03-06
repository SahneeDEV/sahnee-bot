using Microsoft.Extensions.Logging;

namespace SahneeBot.Activity;

/// <summary>
/// Updates the bot activity.
/// </summary>
public class BotActivity
{
    private readonly ILogger<BotActivity> _logger;
    private readonly Bot _bot;

    public BotActivity(ILogger<BotActivity> logger
        , Bot bot)
    {
        _logger = logger;
        _bot = bot;
    }

    /// <summary>
    /// Updates the bot activity
    /// </summary>
    public Task UpdateBotActivity()
    {
        return _bot.ImplAsync(async socket =>
        {
            try
            {
                await socket.SetActivityAsync(new ActivityWatchingGuildsAmount(socket.Guilds.Count));
            }
            catch (Exception e)
            {
                _logger.LogCritical(EventIds.Discord, e, "Could not update the bot activity!");
            }
        }, rest => Task.CompletedTask);
    }
    
}
