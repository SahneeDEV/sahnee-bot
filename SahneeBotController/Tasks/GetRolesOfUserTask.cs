using SahneeBotModel;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gets all roles of the given user.
/// </summary>
public abstract class GetRolesOfUserTask: ITask<GetRolesOfUserTask.Args, IEnumerable<RoleType>>
{
    /// <summary>
    /// Arguments for getting the roles in a guild.
    /// </summary>
    /// <param name="GuildId">The guild of the user.</param>
    /// <param name="UserId">The user ID.</param>
    public record struct Args(ulong GuildId, ulong UserId);
    
    public abstract Task<IEnumerable<RoleType>> Execute(ITaskContext ctx, Args arg);

    /// <summary>
    /// Checks if the user has a role.
    /// </summary>
    /// <param name="ctx">The context.</param>
    /// <param name="arg">The user & guild.</param>
    /// <param name="role">The role to check.</param>
    /// <returns>Does the guild user have the role?</returns>
    public async Task<bool> HasRoleAsync(ITaskContext ctx, Args arg, RoleType role)
    {
        if (role == RoleType.None)
        {
            return true;
        }
        var roles = await Execute(ctx, arg);
        return roles.Contains(role);
    }
}
