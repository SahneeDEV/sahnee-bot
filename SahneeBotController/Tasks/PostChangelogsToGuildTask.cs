namespace SahneeBotController.Tasks;

/// <summary>
/// This task posts the list of given changelog versions to a guild. Returns the amount of changelogs sent.
/// </summary>
public abstract class PostChangelogsToGuildTask : ITask<PostChangelogsToGuildTask.Args, ISuccess<uint>>
{
    /// <summary>
    /// Arguments for this task.
    /// </summary>
    /// <param name="GuildId">The guild ID to post to.</param>
    /// <param name="Versions">The changelog versions to post.</param>
    public record struct Args(ulong GuildId, IEnumerable<Version> Versions);

    public abstract Task<ISuccess<uint>> Execute(ITaskContext ctx, Args arg);
}