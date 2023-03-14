using System.Collections.Concurrent;

namespace SahneeBot;

/// <summary>
/// The queue for guilds that should be checked for changelogs.
/// </summary>
public class GuildChangelogQueue
{
    private readonly ConcurrentQueue<ulong> _queue = new();

    /// <summary>
    /// Enqueues a guild ID.
    /// </summary>
    /// <param name="guildId">The guild ID.</param>
    public void Enqueue(ulong guildId)
    {
        _queue.Enqueue(guildId);
    }

    /// <summary>
    /// Tries to dequeue a guild ID.
    /// </summary>
    /// <param name="guildId">The guild ID, only valid is true is returned.</param>
    /// <returns>If an ID could be dequeued.</returns>
    public bool TryDequeue(out ulong guildId)
    {
        return _queue.TryDequeue(out guildId);
    }
}