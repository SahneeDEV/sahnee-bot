namespace SahneeBotController.Tasks;

public abstract class ChangeRoleColorTask: ITask<ChangeRoleColorTask.Args, string>
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

    public abstract Task<string> Execute(ITaskContext ctx, Args arg);
}