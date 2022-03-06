using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// Prints that the cleanup of the roles failed.
/// </summary>
public class RoleCleanupFailedDiscordFormatter: IDiscordFormatter<RoleCleanupFailedDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    /// <summary>
    /// Arguments for the formatter.
    /// </summary>
    /// <param name="Hint">Why the cleanup failed</param>
    public record struct Args(string Hint);

    public RoleCleanupFailedDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args args)
    {
        var hint = args.Hint;
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = "Cannot cleanup all roles";
        embed.Color = Color.DarkRed;
        embed.Fields = new List<EmbedFieldBuilder>
        {
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