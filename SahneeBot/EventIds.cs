using Microsoft.Extensions.Logging;

namespace SahneeBot;

/// <summary>
/// All log IDs used by the bot.
/// </summary>
public static class EventIds
{
    /// <summary>
    /// Logs related to the bot startup.
    /// </summary>
    public static readonly EventId Startup = new(1, "Startup");
    /// <summary>
    /// Logs related to command handling.
    /// </summary>
    public static readonly EventId Command = new(2, "Command");
    /// <summary>
    /// Discord API log events.
    /// </summary>
    public static readonly EventId Discord = new (3, "Discord");
}