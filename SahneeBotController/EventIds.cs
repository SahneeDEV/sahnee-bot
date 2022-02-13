using Microsoft.Extensions.Logging;

namespace SahneeBotController;

/// <summary>
/// Event IDs of the controller.
/// </summary>
public static class EventIds
{
    /// <summary>
    /// The base offset of all event IDs.
    /// </summary>
    private const int Base = 1000;
    
    /// <summary>
    /// Logs related to warnings.
    /// </summary>
    public static readonly EventId Warning = new(Base + 1, "Warning");
}