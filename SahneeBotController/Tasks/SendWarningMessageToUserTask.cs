using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Send a user a warning message.
/// </summary>
public abstract class SendWarningMessageToUserTask: ITask<SendWarningMessageToUserTask.Args, bool>
{
    private readonly IServiceProvider _provider;

    protected SendWarningMessageToUserTask(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Arguments for sending the message.
    /// </summary>
    public record struct Args(IWarning Warning);

    public Task<bool> Execute(ITaskContext ctx, Args arg)
    {
        return ExecuteImpl(ctx, arg);
    }
    
    protected abstract Task<bool> ExecuteImpl(ITaskContext ctx, Args arg);
}