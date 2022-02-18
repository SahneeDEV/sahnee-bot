using Discord;

namespace SahneeBot.Formatter;

public class GeneralErrorDiscordFormatter: IDiscordFormatter<GeneralErrorDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    public record struct Args(string ErrorTitle, List<EmbedFieldBuilder> Fields);

    public GeneralErrorDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args args)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        var (errorTitle, embedFieldBuilders) = args;
        embed.Title = errorTitle;
        embed.Fields = embedFieldBuilders;

        return Task.FromResult(new DiscordFormat(embed));
    }
}