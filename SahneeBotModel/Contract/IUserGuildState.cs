namespace SahneeBotModel.Contract;

/// <summary>
/// Represents the state of a single user on a single guild.
/// </summary>
public interface IUserGuildState: IGuildSpecific, IUserSpecific
{
    /// <summary>
    /// The amount of warnings the user is on.
    /// </summary>
    uint WarningNumber { get; set; }
    /// <summary>
    /// Has the user opted out of messages from this guild?
    /// </summary>
    bool MessageOptOut { get; set; }
    /// <summary>
    /// Has this user been informed about the possibility of opting out of the private messages.
    /// </summary>
    bool HasReceivedOptOutHint { get; set; }
}
