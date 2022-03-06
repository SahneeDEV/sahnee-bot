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
    
    private readonly Bot _bot;
    private readonly RoleDiscordFormatter _roleFmt;

    public RoleChangedDiscordFormatter(Bot bot
        , RoleDiscordFormatter roleFmt)
    {
        _bot = bot;
        _roleFmt = roleFmt;
    }
    
    public async Task<DiscordFormat> Format(Args arg)
    {
        var (role, roleType, added) = arg;
        if (role == null)
        {
            return new DiscordFormat("The role does not exist.");
        }
        var guild = await _bot.Client.GetGuildAsync(role.GuildId);
        var discordRole = guild.GetRole(role.RoleId);
        if (discordRole == null)
        {
            return new DiscordFormat("The role does not exist.");
        }
        
        var fmt = await _roleFmt.Format(role);
        var addedStr = added ? "Added" : "Removed";
        var title = roleType == null 
            ? $"{addedStr} all sahnee permissions of role \"{discordRole.Name}\""
            : $"{addedStr} sahnee permission {roleType} to role \"{discordRole.Name}\"";
        if (fmt.Embed != null)
        {
            fmt.Embed.Title = title;
        }

        return fmt;
    }
}