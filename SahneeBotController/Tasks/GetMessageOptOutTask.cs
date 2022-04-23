namespace SahneeBotController.Tasks;

/// <summary>
/// Checks if the user has opted out of messages.
/// </summary>
public class GetMessageOptOutTask : ITask<GetMessageOptOutTask.Args, bool>
{
    private readonly GetUserGuildStateTask _getUserGuildStateTask;

    /// <summary>
    /// Arguments for this formatter.
    /// </summary>
    /// <param name="UserId">The user ID that can opt out and should receive the message.</param>
    /// <param name="GuildId">The guild ID the user can opt out of.</param>
    public record struct Args(ulong UserId, ulong GuildId);

    public GetMessageOptOutTask(GetUserGuildStateTask getUserGuildStateTask)
    {
        _getUserGuildStateTask = getUserGuildStateTask;
    }

    public async Task<bool> Execute(ITaskContext ctx, Args arg)
    {
        var state = await _getUserGuildStateTask.Execute(ctx, new GetUserGuildStateTask.Args(arg.GuildId,
            arg.UserId));
        return state.MessageOptOut;
    }
}