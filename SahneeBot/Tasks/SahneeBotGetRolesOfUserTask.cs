using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Tasks;

public class SahneeBotGetRolesOfUserTask: GetRolesOfUserTask
{
    private readonly DiscordSocketClient _bot;
    private static readonly RoleType[] None = Array.Empty<RoleType>();
    private static readonly RoleType[] Admin = { RoleType.Administrator, RoleType.Moderator };
    private static readonly RoleType[] Moderator = { RoleType.Moderator };

    public SahneeBotGetRolesOfUserTask(DiscordSocketClient bot)
    {
        _bot = bot;
    }
    
    public override async Task<IEnumerable<RoleType>> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, userId) = arg;
        var guild = _bot.GetGuild(guildId);
        var user = guild?.GetUser(userId);
        if (user == null)
        {
            return None;
        }

        if (user.GuildPermissions.Administrator)
        {
            return Admin;
        }

        if (user.GuildPermissions.BanMembers)
        {
            return Moderator;
        }

        var userRolesIds = user.Roles.Select(r => r.Id);
        var roles = await ctx.Model.Roles
            .Where(r => r.GuildId == guildId && userRolesIds.Contains(r.RoleId))
            .Select(r => r.RoleType)
            .ToListAsync();

        if (roles.Any(role => (role & RoleType.Administrator) == RoleType.Administrator))
        {
            return Admin;
        }
        
        if (roles.Any(role => (role & RoleType.Moderator) == RoleType.Moderator))
        {
            return Moderator;
        }
        
        return None;
    }
}