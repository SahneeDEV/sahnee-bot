using Microsoft.Extensions.Logging;

namespace SahneeBotModel;

/// <summary>
/// Event IDs of the model.
/// </summary>
public class EventIds
{
    /// <summary>
    /// The base offset of all event IDs.
    /// </summary>
    private const int Base = 2000;
    
    /// <summary>
    /// Logs related to warnings.
    /// </summary>
    public static readonly EventId Model = new(Base + 1, "Model");
}