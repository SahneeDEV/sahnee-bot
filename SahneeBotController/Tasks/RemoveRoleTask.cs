using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Removes the given role from a guild.
/// </summary>
public class RemoveRoleTask: ITask<RemoveRoleTask.Args, IRole?>
{
    public record struct Args(ulong GuildId, string Name);

    public async Task<IRole?> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, name) = arg;
        var role = await ctx.Model.Roles.FirstOrDefaultAsync(role => role.GuildId == guildId
                                                               && role.RoleName == name);
        if (role == null)
        {
            return role;
        }
        
        ctx.Model.Roles.Remove(role);
        await ctx.Model.SaveChangesAsync();

        return role;
    }
}