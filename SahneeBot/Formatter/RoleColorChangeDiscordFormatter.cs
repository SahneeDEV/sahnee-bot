using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// Prints that the color of the roles has been changed.
/// </summary>
public class RoleColorChangeDiscordFormatter : IDiscordFormatter<RoleColorChangeDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _fmt;

    /// <summary>
    /// Arguments for printing the new colorName
    /// </summary>
    /// <param name="ColorName">The color that was changed to.</param>
    public record struct Args(string ColorName);

    public RoleColorChangeDiscordFormatter(DefaultFormatArguments fmt)
    {
        _fmt = fmt;
    }
    
    public Task<DiscordFormat> Format(Args arg)
    {
        var builder = _fmt.GetEmbed();
        builder.Title = "Your Role color changed";
        builder.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "New Color",
                Value = $"Your new color in Hex is: {arg.ColorName}",
                IsInline = false
            }
        };

        return Task.FromResult(new DiscordFormat(builder));
    }
}