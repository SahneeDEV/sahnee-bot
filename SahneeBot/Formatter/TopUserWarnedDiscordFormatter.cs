using Discord.WebSocket;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Formatter;

/// <summary>
/// Formats a single place in the top board.
/// </summary>
public class TopUserWarnedDiscordFormatter: IDiscordFormatter<TopUserWarnedDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;
    private readonly Bot _bot;

    /// <summary>
    /// Arguments for the generation of the format of the warnings top command
    /// </summary>
    /// <param name="Place">The warning place.</param>
    /// <param name="UserId">The user ID.</param>
    /// <param name="WarningNumber">The amount of warnings of the user.</param>
    public record struct Args(uint Place, ulong UserId, uint WarningNumber);
    
    public TopUserWarnedDiscordFormatter(DefaultFormatArguments defaultFormatArguments
        , Bot bot)
    {
        _defaultFormatArguments = defaultFormatArguments;
        _bot = bot;
    }

    public async Task<DiscordFormat> Format(Args arg)
    {
        var (place, userId, warningNumber) = arg;
        var currentUser = await _bot.Client.GetUserAsync(userId);
        var userString = _defaultFormatArguments.GetMention(currentUser);
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