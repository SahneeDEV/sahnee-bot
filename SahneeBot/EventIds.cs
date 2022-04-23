using Microsoft.Extensions.Logging;

namespace SahneeBot;

/// <summary>
/// All log IDs used by the bot.
/// </summary>
public static class EventIds
{
    /// <summary>
    /// The base offset of all event IDs.
    /// </summary>
    private const int BASE = 0;
    
    /// <summary>
    /// Logs related to the bot startup.
    /// </summary>
    public static readonly EventId Startup = new(BASE + 1, "Startup");
    /// <summary>
    /// Logs related to command handling.
    /// </summary>
    public static readonly EventId Command = new(BASE + 2, "Command");
    /// <summary>
    /// Discord API log events.
    /// </summary>
    public static readonly EventId Discord = new (BASE + 3, "Discord");
    /// <summary>
    /// Migration log events.
    /// </summary>
    public static readonly EventId Migration = new (BASE + 4, "Migration");
    /// <summary>
    /// Job log events.
    /// </summary>
    public static readonly EventId Jobs = new(BASE + 5, "Jobs");
    /// <summary>
    /// Task log events.
    /// </summary>
    public static readonly EventId Task = new(BASE + 6, "Task");
    /// <summary>
    /// Context log events.
    /// </summary>
    public static readonly EventId Context = new(BASE + 7, "Context");
}
