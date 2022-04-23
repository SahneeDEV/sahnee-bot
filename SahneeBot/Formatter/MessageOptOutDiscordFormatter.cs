using Discord.WebSocket;

namespace SahneeBot.Formatter;

/// <summary>
/// Formats the current opt out state of messages.
/// </summary>
public class MessageOptOutDiscordFormatter : IDiscordFormatter<MessageOptOutDiscordFormatter.Args>
{
    private readonly Bot _bot;
    private readonly DefaultFormatArguments _fmt;

    /// <summary>
    /// Arguments for this formatter.
    /// </summary>
    /// <param name="UserId">The user ID that can opt out and should receive the message.</param>
    /// <param name="GuildId">The guild ID the user can opt out of.</param>
    /// <param name="OptOut">Has the user opted out?</param>
    public record struct Args(ulong? UserId, ulong? GuildId, bool OptOut);
    
    public MessageOptOutDiscordFormatter(Bot bot
        , DefaultFormatArguments fmt)
    {
        _bot = bot;
        _fmt = fmt;
    }
    
    public async Task<DiscordFormat> Format(Args arg)
    {
        var (userId, guildId, optOut) = arg;
        var user = userId.HasValue ? await _bot.Client.GetUserAsync(userId.Value) : null;
        var guild = guildId.HasValue ? await _bot.Client.GetGuildAsync(guildId.Value) : null;
        var guildStr = guild != null ? $"for {_fmt.GetMention(guild)}" : "for every server";
        var optStr = optOut ? "opted out" : "opted in";
        var userStr = user != null ? $"{_fmt.GetMention(user)} has" : "Default opt-out set to";
        return new DiscordFormat($"{userStr} {optStr} to receiving messages {guildStr}");
    }
}