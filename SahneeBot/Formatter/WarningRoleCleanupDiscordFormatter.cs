using Discord;

namespace SahneeBot.Formatter;

public class WarningRoleCleanupDiscordFormatter : IDiscordFormatter<WarningRoleCleanupDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    /// <summary>
    /// Arguments for this formatter.
    /// </summary>
    /// <param name="RemovedRoles">the amount of roles that have been removed</param>
    public record struct Args(int RemovedRoles);

    public WarningRoleCleanupDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }
    
    public Task<DiscordFormat> Format(Args args)
    {
        var builder = _defaultFormatArguments.GetEmbed();
        builder.Title = args.RemovedRoles > 0 ? "Unused Warning-Roles have been removed!" : "Nothing has been removed," +
            " already everything tidy🧹";
        builder.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Amount removed",
                Value = args.RemovedRoles,
                IsInline = false
            }
        };
        
        return Task.FromResult(new DiscordFormat(builder));
    }
}
