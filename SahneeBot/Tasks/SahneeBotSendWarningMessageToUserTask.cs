using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotSendWarningMessageToUserTask: SendWarningMessageToUserTask
{
    private readonly DiscordSocketClient _bot;
    private readonly ILogger<SahneeBotSendWarningMessageToUserTask> _logger;
    private readonly WarningDiscordFormatter _discordFormatter;

    public SahneeBotSendWarningMessageToUserTask(IServiceProvider provider, DiscordSocketClient bot,
        ILogger<SahneeBotSendWarningMessageToUserTask> logger,
        WarningDiscordFormatter discordFormatter) : base(provider)
    {
        _bot = bot;
        _logger = logger;
        _discordFormatter = discordFormatter;
    }
    
    protected override async Task<bool> ExecuteImpl(ITaskContext ctx, Args arg)
    {
        var (warning, recipientId) = arg;
        // Get the user, could be deleted
        var user = await _bot.GetUserAsync(recipientId);
        if (user == null)
        {
            _logger.LogWarning(EventIds.Discord, 
                "Failed to deliver warning message for warning {Warning}: Could not get the user", warning);
            return false;
        }
        // Sending messages to bots is pointless
        if (user.IsBot)
        {
            return false;
        }
        // Sending the actual message crashes if the bot is blocked
        try
        {
            await _discordFormatter.FormatAndSend(warning, user.SendMessageAsync);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning(EventIds.Discord, e, 
                "Failed to deliver warning message for warning {Warning}", warning);
            return false;
        }
    }
}