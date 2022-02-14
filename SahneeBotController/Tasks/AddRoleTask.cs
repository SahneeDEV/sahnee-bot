using Microsoft.EntityFrameworkCore;
using SahneeBotModel;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// Adds a role. If it already exists with a different role type its role type will be changed.
/// </summary>
public class AddRoleTask: ITask<AddRoleTask.Args, IRole>
{
    public record struct Args(ulong GuildId, string Name, RoleTypes Type);

    public async Task<IRole> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, name, roleTypes) = arg;
        var role = await ctx.Model.Roles.Where(r => r.GuildId == guildId && r.RoleName == name)
            .FirstOrDefaultAsync();
        if (role == null)
        {
            role = new Role
            {
                GuildId = guildId,
                RoleName = name,
                RoleType = roleTypes
            };
            ctx.Model.Roles.Add(role);
            await ctx.Model.SaveChangesAsync();
            return role;
        }

        if (role.RoleType == roleTypes)
        {
            return role;
        }
        
        role.RoleType = roleTypes;
        await ctx.Model.SaveChangesAsync();
        return role;
    }
}