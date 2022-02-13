using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Send a user a unwarning message.
/// </summary>
public abstract class SendUnwarningMessageToUserTask: ITask<SendUnwarningMessageToUserTask.Args, bool>
{
    /// <summary>
    /// Arguments for sending the message.
    /// </summary>
    public struct Args
    {
        /// <summary>
        /// The unwarning to send.
        /// </summary>
        public readonly IWarning Warning;
        /// <summary>
        /// The user ID that will get the message.
        /// </summary>
        public readonly ulong RecipientId;

        public Args(IWarning unwaring)
        {
            Warning = unwaring;
            RecipientId = unwaring.UserId;
        }
    }

    public abstract Task<bool> Execute(ITaskContext ctx, Args arg);
}
