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
    private const int Base = 0;
    
    /// <summary>
    /// Logs related to the bot startup.
    /// </summary>
    public static readonly EventId Startup = new(Base + 1, "Startup");
    /// <summary>
    /// Logs related to command handling.
    /// </summary>
    public static readonly EventId Command = new(Base + 2, "Command");
    /// <summary>
    /// Discord API log events.
    /// </summary>
    public static readonly EventId Discord = new (Base + 3, "Discord");
    /// <summary>
    /// Migration log events.
    /// </summary>
    public static readonly EventId Migration = new (Base + 4, "Migration");
    /// <summary>
    /// Job log events.
    /// </summary>
    public static readonly EventId Jobs = new(Base + 5, "Jobs");
}
