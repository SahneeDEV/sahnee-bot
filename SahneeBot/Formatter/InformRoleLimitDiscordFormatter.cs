using Discord;

namespace SahneeBot.Formatter;

public class InformRoleLimitDiscordFormatter : IDiscordFormatter<InformRoleLimitDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;
    private readonly int DiscordGuildRoleLimit = 250;

    /// <summary>
    /// Arguments for the formatter.
    /// </summary>
    /// <param name="RoleCount">the current role count</param>
    /// <param name="MaxRoleCount">the maximum role count, set by discord</param>
    public record struct Args(int RoleCount);

    public InformRoleLimitDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    
    public Task<DiscordFormat> Format(Args arg)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = "Your roles are close to the limit";
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "You current Role amount",
                Value = arg.RoleCount,
                IsInline = true
            },
            new()
            {
                Name = "Current limit by discord",
                Value = DiscordGuildRoleLimit,
                IsInline = true
            }
        };
        
        return Task.FromResult(new DiscordFormat(embed));
    }
}
