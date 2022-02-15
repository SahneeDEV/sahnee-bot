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
    public record struct Args(ulong GuildId, ulong RoleId, RoleType Type);

    public async Task<IRole> Execute(ITaskContext ctx, Args arg)
    {
        if (arg.Type == RoleType.None)
        {
            throw new InvalidOperationException("Cannot add the \"None\" role type to a role");
        }
        var (guildId, roleId, roleTypes) = arg;
        var role = await ctx.Model.Roles
            .Where(r => r.GuildId == guildId && r.RoleId == roleId)
            .FirstOrDefaultAsync();
        if (role == null)
        {
            role = new Role
            {
                GuildId = guildId,
                RoleId = roleId,
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
        
        role.RoleType |= roleTypes;
        await ctx.Model.SaveChangesAsync();
        return role;
    }
}