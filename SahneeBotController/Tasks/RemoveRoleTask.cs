using Microsoft.EntityFrameworkCore;
using SahneeBotModel;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Removes the given role from a guild.
/// </summary>
public class RemoveRoleTask: ITask<RemoveRoleTask.Args, IRole?>
{
    public record struct Args(ulong GuildId, ulong RoleId, RoleType? RoleType);

    public async Task<IRole?> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, roleId, roleType) = arg;
        var role = await ctx.Model.Roles.FirstOrDefaultAsync(role => role.GuildId == guildId && role.RoleId == roleId);
        if (role == null)
        {
            return role;
        }

        if (roleType == null)
        {
            role.RoleType = RoleType.None;
            ctx.Model.Roles.Remove(role);
        }
        else
        {
            role.RoleType &= ~roleType.Value;
            if (role.RoleType == RoleType.None)
            {
                ctx.Model.Roles.Remove(role);
            }
        }
        
        await ctx.Model.SaveChangesAsync();
        return role;
    }
}