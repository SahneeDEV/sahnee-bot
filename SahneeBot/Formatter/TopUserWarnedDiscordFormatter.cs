using Discord.WebSocket;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Formatter;

public class TopUserWarnedDiscordFormatter: IDiscordFormatter<TopUserWarnedDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;
    private readonly DiscordSocketClient _bot;

    /// <summary>
    /// Arguments for the generation of the format of the warnings top command
    /// </summary>
    public record struct Args(uint Place, ulong UserId, uint WarningNumber);
    
    public TopUserWarnedDiscordFormatter(DefaultFormatArguments defaultFormatArguments, DiscordSocketClient bot)
    {
        _defaultFormatArguments = defaultFormatArguments;
        _bot = bot;
    }

    public async Task<DiscordFormat> Format(Args arg)
    {
        var (place, userId, warningNumber) = arg;
        var currentUser = await _bot.GetUserAsync(userId);
        var userString = currentUser != null ? _defaultFormatArguments.GetMention(currentUser) : "Not Found";
        var emoji = place switch
        {
            1 => "🥇 ",
            2 => "🥈 ",
            3 => "🥉 ",
            _ => ""
        };

        return new DiscordFormat(
            $"{emoji}{place}. {userString}: {warningNumber} warning{(warningNumber == 1 ? ' ' : 's')}\n");
    }
}