using Discord;

namespace SahneeBot.Formatter;

public class WarningRolePrefixChangedDiscordFormatter : IDiscordFormatter<WarningRolePrefixChangedDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    public record struct Args(string NewPrefix);
    
    public WarningRolePrefixChangedDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }
    
    public Task<DiscordFormat> Format(Args arg)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = "Your Warning Role Prefix changed";
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "New Prefix",
                Value = arg.NewPrefix,
                IsInline = false
            }
        };
        return Task.FromResult(new DiscordFormat(embed));
    }
}