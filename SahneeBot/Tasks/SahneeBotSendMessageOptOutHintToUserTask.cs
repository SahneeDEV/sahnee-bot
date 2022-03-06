using Discord;
using Discord.WebSocket;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotSendMessageOptOutHintToUserTask : SendMessageOptOutHintToUserTask
{
    private readonly Bot _bot;
    private readonly MessageOptOutHintDiscordFormatter _fmt;

    public SahneeBotSendMessageOptOutHintToUserTask(IServiceProvider provider
        , Bot bot
        , MessageOptOutHintDiscordFormatter fmt) : base(provider)
    {
        _bot = bot;
        _fmt = fmt;
    }

    protected override async Task<bool> ExecuteImpl(ITaskContext ctx, Args arg)
    {
        var (userId, guildId) = arg;
        var user = await _bot.Client.GetUserAsync(userId);
        if (user == null)
        {
            return false;
        }

        await _fmt.FormatAndSend(new MessageOptOutHintDiscordFormatter.Args(userId, guildId),
            user.SendMessageAsync);
        return true;
    }
}