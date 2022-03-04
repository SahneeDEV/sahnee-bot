using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// This discord formatter prints an error when changing the role prefix.
/// </summary>
public class InvalidPrefixDiscordFormatter: IDiscordFormatter<InvalidPrefixDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    public record struct Args(string Prefix, string Hint);

    public InvalidPrefixDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args args)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        var (prefix, hint) = args;
        embed.Title = "Cannot change role prefix";
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