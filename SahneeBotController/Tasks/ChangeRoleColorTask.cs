namespace SahneeBotController.Tasks;

/// <summary>
/// Changes the role color of the given guild to the given hex color
/// </summary>
public abstract class ChangeRoleColorTask: ITask<ChangeRoleColorTask.Args, ISuccess<string>>
{
    
    public struct Args
    {
        /// <summary>
        /// The guild in which the color will be set
        /// </summary>
        public readonly ulong GuildId;

        /// <summary>
        /// The new color for the roles
        /// </summary>
        public readonly string RoleColor;
    
        public Args(ulong guildId, string roleColor)
        {
            GuildId = guildId;
            RoleColor = roleColor;
        }
    }

    public abstract Task<ISuccess<string>> Execute(ITaskContext ctx, Args arg);
}