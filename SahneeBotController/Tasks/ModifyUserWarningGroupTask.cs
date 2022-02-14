namespace SahneeBotController.Tasks;

public abstract class ModifyUserWarningGroupTask: ITask<ModifyUserWarningGroupTask.Args, bool>
{
    public struct Args
    {
        /// <summary>
        /// The current warning number to set as a role.
        /// </summary>
        public readonly ulong Number;

        /// <summary>
        /// The user ID that will get the warning role.
        /// </summary>
        public readonly ulong UserId;

        /// <summary>
        /// The guild in which the role will be set.
        /// </summary>
        public readonly ulong GuildId;

        public Args(ulong number, ulong userId, ulong guildId)
        {
            Number = number;
            UserId = userId;
            GuildId = guildId;
        }
    }

    public abstract Task<bool> Execute(ITaskContext ctx, Args arg);
}
