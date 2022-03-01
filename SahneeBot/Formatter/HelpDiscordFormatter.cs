using Discord;

namespace SahneeBot.Formatter;

public class HelpDiscordFormatter: IDiscordFormatter<HelpDiscordFormatter.Args>
{
    public record struct Args();
    
    private readonly DefaultFormatArguments _defaultFormatArguments;
    private readonly string _website = "https://sahnee.dev/en/project/sahnee-bot/";
    private readonly string _github = "https://github.com/Sahnee-DE/sahnee-bot";

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
                Value = _website,
                IsInline = true
            },
            new()
            {
                Name = "GitHub Repository of the bot",
                Value = _github,
                IsInline = true
            }
        };
        
        return Task.FromResult(new DiscordFormat(embed));
    }
}