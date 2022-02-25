using Discord;

namespace SahneeBot.Formatter;

public class WarningRoleSetDiscordFormatter : IDiscordFormatter<WarningRoleSetDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    /// <summary>
    /// Arguments for printing the warning role set/unset or show
    /// </summary>
    /// <param name="State"></param>
    public record struct Args(bool State, string Prefix, bool Changed);

    public WarningRoleSetDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args args)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = args.Changed ? "Warning-Roles set setting has been changed" :
            "Your current Warning-Roles configuration";
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = args.Changed? "New configuration" : "Current configuration",
                Value = "Warning/Unwarning roles " + 
                        (args.State ? "will" : "will not") + " be set",
                IsInline = true
            },
            new()
            {
                Name = "Current Prefix",
                Value = args.Prefix.Replace(" ", ""),
                IsInline = true
            }
        };
        return Task.FromResult(new DiscordFormat(embed));
    }
}
