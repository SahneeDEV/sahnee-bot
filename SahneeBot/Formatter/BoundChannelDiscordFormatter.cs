using Discord;
using Discord.WebSocket;

namespace SahneeBot.Formatter;

/// <summary>
/// Prints information about the bound channel for a guild.
/// </summary>
public class BoundChannelDiscordFormatter : IDiscordFormatter<BoundChannelDiscordFormatter.Args>
{
    /// <summary>
    /// Arguments for the formatter.
    /// </summary>
    /// <param name="GuildId">The guild ID.</param>
    /// <param name="BoundChannelId">The bound channel.</param>
    public record struct Args(ulong? GuildId, ulong? BoundChannelId);
    
    private readonly Bot _bot;

    public BoundChannelDiscordFormatter(Bot bot) => _bot = bot;

    public async Task<DiscordFormat> Format(Args arg)
    {
        var (guildId, boundChannelId) = arg;
        var guild = guildId.HasValue ? await _bot.Client.GetGuildAsync(guildId.Value) : null;
        var channel = boundChannelId.HasValue ? await _bot.Client.GetChannelAsync(boundChannelId.Value) : null;
        var mentionable = channel as IMentionable;
        var guildPrefix = guild == null ? "Global commands have" : $"{guild.Name} has";
        return channel == null
            ? new DiscordFormat($"{guildPrefix} not been bound to a channel.")
            : new DiscordFormat($"{guildPrefix} been bound to channel {mentionable?.Mention ?? channel.Name}.");
    }
}