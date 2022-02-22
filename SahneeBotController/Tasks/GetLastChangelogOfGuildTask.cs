namespace SahneeBotController.Tasks;

/// <summary>
/// Gets the last changelog this guild has received.
/// </summary>
public class GetLastChangelogOfGuildTask : ITask<GetLastChangelogOfGuildTask.Args, Version?>
{
    private readonly GetGuildStateTask _stateTask;

    /// <summary>
    /// Arguments for the task.
    /// </summary>
    /// <param name="GuildId">The guild ID.</param>
    public record struct Args(ulong GuildId);

    public GetLastChangelogOfGuildTask(GetGuildStateTask stateTask)
    {
        _stateTask = stateTask;
    }

    public async Task<Version?> Execute(ITaskContext ctx, Args arg)
    {
        var state = await _stateTask.Execute(ctx, new GetGuildStateTask.Args(arg.GuildId));
        return state.LastChangelogVersion;
    }
}