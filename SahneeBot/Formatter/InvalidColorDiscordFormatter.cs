using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// This discord formatter prints an error when changing the role colors.
/// </summary>
public class InvalidColorDiscordFormatter : IDiscordFormatter<InvalidColorDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    public record struct Args(string Color, string Hint);

    public InvalidColorDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args args)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        var (color, hint) = args;
        embed.Title = "Cannot change role color";
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "You specified color",
                Value = color,
                IsInline = true
            },
            new()
            {
                Name = "Hint",
                Value = hint,
                IsInline = false
            }
        };

        return Task.FromResult(new DiscordFormat(embed));
    }
}