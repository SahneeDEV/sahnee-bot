using Discord;
using SahneeBotModel;

namespace SahneeBot.Formatter;

/// <summary>
/// Prints that a user could not be warned.
/// </summary>
public class FailedToWarnDiscordFormatter : IDiscordFormatter<FailedToWarnDiscordFormatter.Args>
{
    private readonly Bot _bot;
    private readonly DefaultFormatArguments _fmt;

    /// <summary>
    /// Format arguments.
    /// </summary>
    /// <param name="GuildId">The guild the warning was on.</param>
    /// <param name="UserId">The user that should be warned.</param>
    /// <param name="Type">The warning type.</param>
    /// <param name="Hint">An error hint.</param>
    public record struct Args(ulong GuildId, ulong UserId, WarningType Type, string Hint);
    
    public FailedToWarnDiscordFormatter(Bot bot
        , DefaultFormatArguments fmt)
    {
        _bot = bot;
        _fmt = fmt;
    }

    public async Task<DiscordFormat> Format(Args arg)
    {
        var embed = _fmt.GetEmbed();
        var (guildId, userId, type, hint) = arg;
        var guild = await _bot.Client.GetGuildAsync(guildId);
        var user = await _bot.Client.GetUserAsync(guildId);
        var typeStr = type == WarningType.Warning ? "warning" : "unwarning";
        embed.Title = $"Cannot issue {typeStr}";
        embed.Color = Color.DarkRed;
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Warned user",
                Value = _fmt.GetMention(user),
                IsInline = true
            },
            new()
            {
                Name = "Server",
                Value = _fmt.GetMention(guild),
                IsInline = true
            },
            new()
            {
                Name = "Hint",
                Value = hint,
                IsInline = false
            }
        };

        return new DiscordFormat(embed);
    }
}