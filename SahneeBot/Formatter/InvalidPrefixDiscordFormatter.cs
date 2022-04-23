using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// This discord formatter prints an error when changing the role prefix.
/// </summary>
public class InvalidPrefixDiscordFormatter: IDiscordFormatter<InvalidPrefixDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    /// <summary>
    /// Arguments for the formatter.
    /// </summary>
    /// <param name="Prefix">The attempted prefix.</param>
    /// <param name="Hint">Why setting the prefix failed.</param>
    public record struct Args(string Prefix, string Hint);

    public InvalidPrefixDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args args)
    {
        var (prefix, hint) = args;
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = "Cannot change role prefix";
        embed.Color = Color.DarkRed;
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "You specified prefix",
                Value = prefix,
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