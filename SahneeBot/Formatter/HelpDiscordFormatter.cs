using Discord;

namespace SahneeBot.Formatter;

public class HelpDiscordFormatter: IDiscordFormatter<HelpDiscordFormatter.Args>
{
    /// <summary>
    /// Arguments for the generation of the format of the help command
    /// </summary>
    public struct Args
    {
        /// <summary>
        /// The website of the sahnee bot
        /// </summary>
        public readonly string Website = "https://sahnee.dev/en/project/sahnee-bot/";
        /// <summary>
        /// The github repo address of the sahnee bot
        /// </summary>
        public readonly string Github = "https://github.com/Sahnee-DE/sahnee-bot";
    }
    
    private readonly DefaultFormatArguments _defaultFormatArguments;

    public HelpDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args arg)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = "More details about this bot:";
        embed.Fields = new List<EmbedFieldBuilder>()
        {
            new EmbedFieldBuilder()
            {
                Name = "Website of the bot",
                Value = arg.Website,
                IsInline = true
            },
            new EmbedFieldBuilder()
            {
                Name = "GitHub Repository of the bot",
                Value = arg.Github,
                IsInline = true
            }
        };
        
        return Task.FromResult(new DiscordFormat(embed.Build()));
    }
}