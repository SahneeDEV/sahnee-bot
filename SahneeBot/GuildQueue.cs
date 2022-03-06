using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SahneeBot;

public delegate Task GuildQueueDelegate();

/// <summary>
/// Contains the FIFO queues for all guilds.
/// </summary>
public class GuildQueue
{
    /// <summary>
    /// Guild specific data in the queue.
    /// </summary>
    private class GuildSpecific
    {
        /// <summary>
        /// The interval in which to re check the queue.
        /// </summary>
        private const int IntervalMs = 100;
        
        private readonly GuildQueue _guildQueue;
        private readonly ulong _guildId;
        private readonly Thread _thread;
        private readonly ConcurrentQueue<GuildQueueDelegate?> _queue = new();
        private bool _running;

        /// <summary>
        /// Creates the queue for the given guild.
        /// </summary>
        /// <param name="guildQueue">The wrapping guild queue.</param>
        /// <param name="guildId">The guild ID.</param>
        public GuildSpecific(GuildQueue guildQueue, ulong guildId)
        {
            _guildQueue = guildQueue;
            _guildId = guildId;
            _thread = new Thread(Loop)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            Start();
        }

        /// <summary>
        /// Starts the queue.
        /// </summary>
        public void Start()
        {
            _running = true;
            _thread.Start();
        }

        /// <summary>
        /// Stops the queue.
        /// </summary>
        public void Stop()
        {
            _running = false;
        }

        /// <summary>
        /// Enqueues a new delegate.
        /// </summary>
        /// <param name="del">The delegate.</param>
        public void Enqueue(GuildQueueDelegate? del)
        {
            _queue.Enqueue(del);
        }

        /// <summary>
        /// The main thread loop.
        /// </summary>
        private async void Loop()
        {
            _guildQueue._logger.LogInformation("Started queue for guild {GuildId}", _guildId);
            // Keep alive for the guild
            while (_running)
            {
                // Get the current first item in the FiFo queue
                if (_queue.TryDequeue(out var del) && del != null)
                {
                    _guildQueue._logger.LogDebug("Dequeued delegate for guild {GuildId}", _guildId);
                    try
                    {
                        await del();
                    }
                    catch (Exception e)
                    {
                        _guildQueue._logger.LogError(e, "Failed to execute delegate in guild {GuildId}", _guildId);
                    }
                }
                Thread.Sleep(IntervalMs);
            }
        }
    }
    
    private readonly ConcurrentDictionary<ulong, GuildSpecific> _guilds = new();
    private readonly ILogger<GuildQueue> _logger;

    /// <summary>
    /// Creates a new Multithreaded FiFo Queue for the commands
    /// </summary>
    public GuildQueue(ILogger<GuildQueue> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// The factory to create a queue for a given guild.
    /// </summary>
    /// <param name="guildId">The guild ID.</param>
    /// <returns>The queue.</returns>
    private GuildSpecific GuildFactory(ulong guildId) => new(this, guildId);
    
    /// <summary>
    /// Gets or add guild data for the given guild.
    /// </summary>
    /// <param name="guildId">The guild ID.</param>
    /// <returns>The guild data.</returns>
    private GuildSpecific GetOrAddGuild(ulong guildId) => _guilds.GetOrAdd(guildId, GuildFactory);

    /// <summary>
    /// Adds a commands execution into the queue
    /// </summary>
    /// <param name="guildId">The guild.</param>
    /// <param name="task">The task to enqueue.</param>
    public void Enqueue(ulong guildId, GuildQueueDelegate? task)
    {
        _logger.LogTrace("Enqueueing a message for guild {Guild}", guildId);
        var guild = GetOrAddGuild(guildId);
        guild.Enqueue(task);
    }
}