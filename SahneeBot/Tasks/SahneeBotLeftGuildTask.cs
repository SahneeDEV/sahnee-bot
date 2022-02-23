using Discord;
using SahneeBot.Activity;

namespace SahneeBot.Tasks;

public class SahneeBotLeftGuildTask
{
    private readonly BotActivity _botActivity;

    public SahneeBotLeftGuildTask(BotActivity botActivity)
    {
        _botActivity = botActivity;
    }

    public async Task LeftGuildTask(IGuild? guild)
    {
        await _botActivity.UpdateBotActivity();
    }
}
