using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotSendWarningMessageToUserTask: SendWarningMessageToUserTask
{
    private readonly GetUserGuildStateTask _getUserGuildStateTask;
    private readonly DiscordSocketClient _bot;
    private readonly ILogger<SahneeBotSendWarningMessageToUserTask> _logger;
    private readonly WarningDiscordFormatter _discordFormatter;

    public SahneeBotSendWarningMessageToUserTask(
        GetUserGuildStateTask getUserGuildStateTask, DiscordSocketClient bot, 
        ILogger<SahneeBotSendWarningMessageToUserTask> logger, WarningDiscordFormatter discordFormatter)
    {
        _getUserGuildStateTask = getUserGuildStateTask;
        _bot = bot;
        _logger = logger;
        _discordFormatter = discordFormatter;
    }
    
    public override async Task<bool> Execute(ITaskContext ctx, Args arg)
    {
        // Don't send a message if the user opted out.
        var userGuildState = await _getUserGuildStateTask.Execute(
            ctx, 
            new GetUserGuildStateTask.Args(arg.Warning.GuildId, arg.Warning.UserId));
        if (userGuildState.MessageOptOut)
        {
            return false;
        }
        // Get the user, could be deleted
        var user = await _bot.GetUserAsync(arg.Warning.UserId);
        if (user == null)
        {
            _logger.LogWarning(EventIds.Discord, 
                "Failed to deliver warning message for warning {warning}: Could not get the user", 
                arg.Warning);
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
            await _discordFormatter.FormatAndSend(arg.Warning, user.SendMessageAsync);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning(EventIds.Discord,
                e,
                "Failed to deliver warning message for warning {warning}", 
                arg.Warning);
            return false;
        }
    }
}