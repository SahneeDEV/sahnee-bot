using Discord;
using Microsoft.Extensions.Configuration;

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
    private readonly string _website;
    private readonly string _github;

    public HelpDiscordFormatter(DefaultFormatArguments defaultFormatArguments
        , Release release
        , Changelog changelog
        , IConfiguration configuration)
    {
        _defaultFormatArguments = defaultFormatArguments;
        _release = release;
        _changelog = changelog;
        _website = configuration["BotSettings:Url"];
        _github = configuration["BotSettings:GithubUrl"];
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
                Value = _website,
                IsInline = true
            },
            new()
            {
                Name = "GitHub Repository of the bot",
                Value = _github,
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