namespace SahneeBotController.Tasks;

/// <summary>
/// Interface for a task.
/// </summary>
/// <typeparam name="TArg">The task argument.</typeparam>
/// <typeparam name="TRes">The result type.</typeparam>
public interface ITask<in TArg, TRes>
{
    /// <summary>
    /// Executes the task.
    /// </summary>
    /// <param name="ctx">The context.</param>
    /// <param name="arg">Arguments for the task.</param>
    /// <returns>The task result.</returns>
    Task<TRes> Execute(ITaskContext ctx, TArg arg);
}
