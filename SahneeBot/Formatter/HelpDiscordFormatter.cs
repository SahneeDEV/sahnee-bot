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
    public record struct Args();
    
    private readonly DefaultFormatArguments _defaultFormatArguments;
    private const string WEBSITE = "https://sahnee.dev/en/project/sahnee-bot/";
    private const string GITHUB = "https://github.com/Sahnee-DE/sahnee-bot";

    public HelpDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
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
            }
        };
        
        return Task.FromResult(new DiscordFormat(embed));
    }
}