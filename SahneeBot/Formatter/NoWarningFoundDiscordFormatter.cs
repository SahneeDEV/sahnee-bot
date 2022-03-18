using Discord.WebSocket;

namespace SahneeBot.Formatter;

/// <summary>
/// Prints that the given guild/optional user combination does not have any warnings.
/// </summary>
public class NoWarningFoundDiscordFormatter: IDiscordFormatter<NoWarningFoundDiscordFormatter.Args>
{
    private readonly Bot _bot;
    private readonly DefaultFormatArguments _fmt;

    public record struct Args(ulong GuildId, ulong? UserId, bool Issuer);
    
    public NoWarningFoundDiscordFormatter(Bot bot
        , DefaultFormatArguments fmt)
    {
        _bot = bot;
        _fmt = fmt;
    }
    
    public async Task<DiscordFormat> Format(Args arg)
    {
        var (guildId, userId, issuer) = arg;
        var guild = await _bot.Client.GetGuildAsync(guildId);
        if (guild == null)
        {
            return new DiscordFormat("No warnings found.");
        }
        var user = userId.HasValue ? await guild.GetGuildUserAsync(userId.Value) : null;
        if (user == null)
        {
            return new DiscordFormat($"No warnings found on {_fmt.GetMention(guild)}.");
        }
        var issuerStr = issuer ? "issued by" : "issued to";
        return new DiscordFormat($"No warnings found on {_fmt.GetMention(guild)} {issuerStr} " +
                                 $"{_fmt.GetMention(user)}.");
    }
}