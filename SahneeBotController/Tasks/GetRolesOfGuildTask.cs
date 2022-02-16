using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gets all roles configured in the given guild.
/// </summary>
public class GetRolesOfGuildTask : ITask<ulong, IEnumerable<IRole>>
{
    public async Task<IEnumerable<IRole>> Execute(ITaskContext ctx, ulong arg)
    {
        return await ctx.Model.Roles.Where(role => role.GuildId == arg).ToListAsync<IRole>();
    }
}