using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Send a user a warning message.
/// </summary>
public abstract class SendWarningMessageToUserTask: ITask<SendWarningMessageToUserTask.Args, bool>
{
    public class Args
    {
        public IWarning Warning;

        public Args(IWarning warning)
        {
            Warning = warning;
        }
    }

    public abstract Task<bool> Execute(ITaskContext ctx, Args arg);
}