using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Tasks;

public class SahneeBotGetRolesOfUserTask: GetRolesOfUserTask
{
    private readonly DiscordSocketClient _bot;
    private static readonly RoleTypes[] None = Array.Empty<RoleTypes>();
    private static readonly RoleTypes[] Admin = { RoleTypes.Administrator, RoleTypes.Moderator };
    private static readonly RoleTypes[] Moderator = { RoleTypes.Moderator };

    public SahneeBotGetRolesOfUserTask(DiscordSocketClient bot)
    {
        _bot = bot;
    }
    
    public override async Task<IEnumerable<RoleTypes>> Execute(ITaskContext ctx, Args arg)
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

        var userRolesNames = user.Roles.Select(r => r.Name);
        var roles = await ctx.Model.Roles
            .Where(r => r.GuildId == guildId && userRolesNames.Contains(r.RoleName))
            .Select(r => r.RoleType)
            .ToListAsync();

        if (roles.Contains(RoleTypes.Administrator))
        {
            return Admin;
        }
        
        if (roles.Contains(RoleTypes.Moderator))
        {
            return Moderator;
        }
        
        return None;
    }
}