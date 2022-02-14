using Discord;

namespace SahneeBot.Formatter;

public class HelpDiscordFormatter: IDiscordFormatter<HelpDiscordFormatter.Args>
{
    /// <summary>
    /// Arguments for the generation of the format of the help command
    /// </summary>
    public record struct Args(string Website = "https://sahnee.dev/en/project/sahnee-bot/",
        string Github = "https://github.com/Sahnee-DE/sahnee-bot");
    
    private readonly DefaultFormatArguments _defaultFormatArguments;

    public HelpDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args arg)
    {
        var (website, github) = arg;
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = "More details about this bot:";
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Website of the bot",
                Value = website,
                IsInline = true
            },
            new()
            {
                Name = "GitHub Repository of the bot",
                Value = github,
                IsInline = true
            }
        };
        
        return Task.FromResult(new DiscordFormat(embed.Build()));
    }
}