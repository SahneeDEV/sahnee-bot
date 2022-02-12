using Discord.WebSocket;
using SahneeBotModel.Contract;

namespace SahneeBot.Formatter;

public class WarningDiscordFormatter: IDiscordFormatter<IWarning>
{
    private readonly DiscordSocketClient _bot;

    public WarningDiscordFormatter(DiscordSocketClient bot)
    {
        _bot = bot;
    }
    
    public Task<DiscordFormat> Format(IWarning arg)
    {
        var guild = _bot.GetGuild(arg.GuildId);
        return Task.FromResult(new DiscordFormat
            {
                Text = $":thumbsdown: [{guild.Name}] <@{arg.UserId}> has been warned by <@{arg.IssuerUserId}> at {arg.Time}. This is warning #{arg.Number}. (Reason: {arg.Reason})"
            });
    }
}