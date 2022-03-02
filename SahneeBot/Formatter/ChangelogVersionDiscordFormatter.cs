using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// Formats a changelog version.
/// </summary>
public class ChangelogVersionDiscordFormatter : IDiscordFormatter<Changelog.VersionInformation>
{
    private readonly DefaultFormatArguments _fmt;

    public ChangelogVersionDiscordFormatter(DefaultFormatArguments fmt)
    {
        _fmt = fmt;
    }

    public Task<DiscordFormat> Format(Changelog.VersionInformation arg)
    {
        var embed = _fmt.GetEmbed();
        embed.Title = "Sahnee-Bot Version " + arg.Version;
        embed.Description = arg.Description;
        embed.Fields = arg.Sections.Select(section => new EmbedFieldBuilder
        {
            Name = section.Name,
            Value = string.IsNullOrWhiteSpace(section.Content) ? section.Name : section.Content
        }).ToList();
        return Task.FromResult(new DiscordFormat(embed));
    }
}