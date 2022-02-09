namespace SahneeBotModel.Contract;

/// <summary>
/// A warning or unwarning for the Sahnee bot.
/// </summary>
public interface IWarning: ISnowflake, IGuildSpecific
{
    /// <summary>
    /// The issue time of the warning.
    /// </summary>
    DateTime Time { get; set; }
    /// <summary>
    /// The user that issued the warning.
    /// </summary>
    ulong From { get; set; }
    /// <summary>
    /// The user that the warning was issued to.
    /// </summary>
    ulong To { get; set; }
    /// <summary>
    /// The reason the warning was issued for.
    /// </summary>
    string Reason { get; set; }
    /// <summary>
    /// The number of warnings the user was on after this warning.
    /// </summary>
    ulong Number { get; set; }
}
