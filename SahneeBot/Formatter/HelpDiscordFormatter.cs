using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// Formats the bot help.
/// </summary>
public class HelpDiscordFormatter: IDiscordFormatter<HelpDiscordFormatter.Args>
{
    /// <summary>
    /// Arguments for the help formatter.
    /// </summary>
    public record struct Args(ulong? GuildId, ulong? UserId);
    
    private readonly DefaultFormatArguments _defaultFormatArguments;
    private readonly Release _release;
    private readonly Changelog _changelog;
    private const string WEBSITE = "https://sahnee.dev/en/project/sahnee-bot/";
    private const string GITHUB = "https://github.com/Sahnee-DE/sahnee-bot";

    public HelpDiscordFormatter(DefaultFormatArguments defaultFormatArguments
        , Release release
        , Changelog changelog)
    {
        _defaultFormatArguments = defaultFormatArguments;
        _release = release;
        _changelog = changelog;
    }

    public Task<DiscordFormat> Format(Args arg)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = "More details about this bot:";
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Website of the bot",
                Value = WEBSITE,
                IsInline = true
            },
            new()
            {
                Name = "GitHub Repository of the bot",
                Value = GITHUB,
                IsInline = true
            },
            new()
            {
                Name = "Bot uptime since",
                Value = _release.StartedAt,
                IsInline = true
            },
            new()
            {
                Name = "Bot version",
                Value = _changelog.Versions.Max(v => v.Version),
                IsInline = true
            },
            new()
            {
                Name = "Server ID",
                Value =  "`" + arg.GuildId + "`",
                IsInline = true
            },
            new()
            {
                Name = "User ID",
                Value = "`" + arg.UserId + "`",
                IsInline = true
            },
            new()
            {
                Name = "Release information",
                Value = "```\n" + _release.Data + "\n```",
                IsInline = false
            },
        };
        
        return Task.FromResult(new DiscordFormat(embed));
    }
}