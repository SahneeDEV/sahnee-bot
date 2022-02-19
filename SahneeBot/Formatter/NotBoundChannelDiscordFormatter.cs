using Discord.WebSocket;

namespace SahneeBot.Formatter;

/// <summary>
/// This discord formatter prints a message informing about the user using the wrong channel.
/// </summary>
public class NotBoundChannelDiscordFormatter : IDiscordFormatter<NotBoundChannelDiscordFormatter.Args>
{
    /// <summary>
    /// Arguments for the formatter.
    /// </summary>
    /// <param name="GuildId">The guild ID that was using the wrong channel.</param>
    /// <param name="BoundChannelId">The actual bound channel.</param>
    public record struct Args(ulong? GuildId, ulong? BoundChannelId);
    
    private readonly DiscordSocketClient _bot;
    private readonly BoundChannelDiscordFormatter _fmt;

    public NotBoundChannelDiscordFormatter(DiscordSocketClient bot, BoundChannelDiscordFormatter fmt)
    {
        _bot = bot;
        _fmt = fmt;
    }

    public async Task<DiscordFormat> Format(Args arg)
    {
        var (guildId, boundChannelId) = arg;
        return await _fmt.Format(new BoundChannelDiscordFormatter.Args(guildId, boundChannelId));
    }
}