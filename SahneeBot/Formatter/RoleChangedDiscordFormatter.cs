using Discord;
using Discord.WebSocket;
using SahneeBotModel;
using IRole = SahneeBotModel.Contract.IRole;

namespace SahneeBot.Formatter;

/// <summary>
/// Formats a role.
/// </summary>
public class RoleChangedDiscordFormatter : IDiscordFormatter<RoleChangedDiscordFormatter.Args>
{
    /// <summary>
    /// Arguments for printing the change in the role.
    /// </summary>
    /// <param name="Role">The role.</param>
    /// <param name="Changed">The role type that changed.</param>
    /// <param name="Added">Was the role added (true) or removed (false)?</param>
    public record struct Args(IRole? Role, RoleType? Changed, bool Added);
    
    private static readonly RoleType[] Values = Enum.GetValues<RoleType>();
    private static readonly RoleType[] None = { RoleType.None };
    
    private readonly DefaultFormatArguments _fmt;
    private readonly DiscordSocketClient _bot;

    public RoleChangedDiscordFormatter(DefaultFormatArguments fmt, DiscordSocketClient bot)
    {
        _fmt = fmt;
        _bot = bot;
    }
    
    public Task<DiscordFormat> Format(Args arg)
    {
        var (role, roleType, added) = arg;
        if (role == null)
        {
            return Task.FromResult(new DiscordFormat("The role does not exist."));
        }
        var builder = _fmt.GetEmbed();
        var guild = _bot.GetGuild(role.GuildId);
        var discordRole = guild.GetRole(role.RoleId);
        if (discordRole == null)
        {
            return Task.FromResult(new DiscordFormat("The role does not exist."));
        }
        var roles = RolesIn(role.RoleType).ToArray();
        var addedStr = added ? "Added" : "Removed";
        var title = roleType == null 
            ? $"{addedStr} all sahnee permissions of role \"{discordRole.Name}\""
            : $"{addedStr} sahnee permission {roleType} to role \"{discordRole.Name}\"";
        builder.Title = title;
        builder.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Role",
                Value = discordRole.Mention,
                IsInline = true
            },
            new()
            {
                Name = roles.Length == 1 ? "Sahnee permission" : "Sahnee permissions",
                Value = string.Join(", ", roles),
                IsInline = true
            },
            new()
            {
                Name = "Server",
                Value = guild.Name,
                IsInline = true
            },
        };

        return Task.FromResult(new DiscordFormat(builder.Build()));
    }

    private static IEnumerable<RoleType> RolesIn(RoleType roleType)
    {
        var roles = Values
            .Where(value => (roleType & value) == value && value != RoleType.None)
            .ToArray();
        return roles.Length == 0 ? None : roles;
    }
}