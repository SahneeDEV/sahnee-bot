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
    public record struct Args(IWarning Warning, ulong RecipientId);

    public abstract Task<bool> Execute(ITaskContext ctx, Args arg);
}