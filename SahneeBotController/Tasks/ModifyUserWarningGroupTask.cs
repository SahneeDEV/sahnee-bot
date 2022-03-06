namespace SahneeBotController.Tasks;

/// <summary>
/// Sets a given warning role to a given user. Returns the new role ID.
/// </summary>
public abstract class ModifyUserWarningGroupTask: ITask<ModifyUserWarningGroupTask.Args, ISuccess<ulong>>
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

    public abstract Task<ISuccess<ulong>> Execute(ITaskContext ctx, Args arg);
}
