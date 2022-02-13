using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Send a user a warning message.
/// </summary>
public abstract class SendWarningMessageToUserTask: ITask<SendWarningMessageToUserTask.Args, bool>
{
    /// <summary>
    /// Arguments for sending the message.
    /// </summary>
    public struct Args
    {
        /// <summary>
        /// The warning to send.
        /// </summary>
        public readonly IWarning Warning;
        /// <summary>
        /// The user ID that will get the message.
        /// </summary>
        public readonly ulong RecipientId;

        public Args(IWarning warning)
        {
            Warning = warning;
            RecipientId = warning.UserId;
        }
    }

    public abstract Task<bool> Execute(ITaskContext ctx, Args arg);
}