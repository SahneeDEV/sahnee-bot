namespace SahneeBotController.Tasks;

/// <summary>
/// This task posts the list of given changelog versions to a guild. Returns if the message could be posted.
/// </summary>
public abstract class PostChangelogsToGuildTask : ITask<PostChangelogsToGuildTask.Args, bool>
{
    /// <summary>
    /// Arguments for this task.
    /// </summary>
    /// <param name="GuildId">The guild ID to post to.</param>
    /// <param name="Versions">The changelog versions to post.</param>
    public record struct Args(ulong GuildId, IEnumerable<Version> Versions);

    public abstract Task<bool> Execute(ITaskContext ctx, Args arg);
}