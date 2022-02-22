using Discord.WebSocket;

namespace SahneeBot.Formatter;

/// <summary>
/// Prints that no changelog for the optionally given version is available.
/// </summary>
public class NoChangelogFoundDiscordFormatter : IDiscordFormatter<NoChangelogFoundDiscordFormatter.Args>
{
    private readonly DiscordSocketClient _bot;
    private readonly DefaultFormatArguments _fmt;

    /// <summary>
    /// Arguments for the formatter.
    /// </summary>
    /// <param name="GuildId">The guild ID.</param>
    /// <param name="Version">An optional changelog version.</param>
    /// <param name="All">All versions after this version too?.</param>
    public record struct Args(ulong? GuildId, Version? Version, bool All);
    
    public NoChangelogFoundDiscordFormatter(DiscordSocketClient bot, DefaultFormatArguments fmt)
    {
        _bot = bot;
        _fmt = fmt;
    }

    public Task<DiscordFormat> Format(Args arg)
    {
        var (guildId, version, all) = arg;
        var guild = guildId == null ? null : _bot.GetGuild(guildId.Value);
        var guildStr = guild == null ? "" : $" on {_fmt.GetMention(guild)}";
        var allStr = all ? "after" : "for";
        var changelogStr = all ? "changelogs" : "changelog";
        var versionStr = version == null ? "" : $" {allStr} version {version}";
        return Task.FromResult(new DiscordFormat($"No {changelogStr} available{versionStr}{guildStr}."));
    }
}