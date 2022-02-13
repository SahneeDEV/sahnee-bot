using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotSendUnwarningMessageToUserTask: SendUnwarningMessageToUserTask
{
    private readonly GetUserGuildStateTask _userGuildState;
    private readonly DiscordSocketClient _bot;
    private readonly ILogger<SahneeBotSendUnwarningMessageToUserTask> _logger;
    private readonly UnwarningDiscordFormatter _discordFormatter;

    public SahneeBotSendUnwarningMessageToUserTask(
        GetUserGuildStateTask userGuildState, DiscordSocketClient bot, 
        ILogger<SahneeBotSendUnwarningMessageToUserTask> logger, UnwarningDiscordFormatter discordFormatter)
    {
        _userGuildState = userGuildState;
        _bot = bot;
        _logger = logger;
        _discordFormatter = discordFormatter;
    }
    
    public override async Task<bool> Execute(ITaskContext ctx, Args arg)
    {
        // Don't send a message if the user opted out.
        var userGuildState = await _userGuildState.Execute(
            ctx, 
            new GetUserGuildStateTask.Args(arg.Warning.GuildId, arg.Warning.UserId));
        if (userGuildState.MessageOptOut)
        {
            return false;
        }
        // Get the user, could be deleted
        var user = await _bot.GetUserAsync(arg.RecipientId);
        if (user == null)
        {
            _logger.LogWarning(EventIds.Discord, 
                "Failed to deliver unwarning message for warning {warning}: Could not get the user", 
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
                "Failed to deliver unwarning message for unwarning {warning}", 
                arg.Warning);
            return false;
        }
    }
}