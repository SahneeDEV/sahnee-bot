using Discord.WebSocket;

namespace SahneeBot.Formatter;

/// <summary>
/// Prints that the given guild/optional user combination does not have any warnings.
/// </summary>
public class NoWarningFoundDiscordFormatter: IDiscordFormatter<NoWarningFoundDiscordFormatter.Args>
{
    private readonly DiscordSocketClient _bot;

    public record struct Args(ulong GuildId, ulong? UserId, bool Issuer);
    
    public NoWarningFoundDiscordFormatter(DiscordSocketClient bot)
    {
        _bot = bot;
    }
    
    public Task<DiscordFormat> Format(Args arg)
    {
        var (guildId, userId, issuer) = arg;
        var guild = _bot.GetGuild(guildId);
        if (guild == null)
        {
            return Task.FromResult(new DiscordFormat("No warnings found."));
        }
        var user = userId.HasValue ? guild.GetUser(userId.Value) : null;
        if (user == null)
        {
            return Task.FromResult(new DiscordFormat($"No warnings found on *{guild.Name}*."));
        }
        var issuerStr = issuer ? "issued by" : "issued to";
        return Task.FromResult(new DiscordFormat($"No warnings found on *{guild.Name}* {issuerStr} {user.Mention}."));
    }
}