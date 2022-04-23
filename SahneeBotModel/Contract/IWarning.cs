namespace SahneeBotModel.Contract;

/// <summary>
/// A warning or unwarning for the Sahnee bot.
/// </summary>
public interface IWarning: ISnowflake, IGuildSpecific, IUserSpecific
{
    /// <summary>
    /// The issue time of the warning.
    /// </summary>
    DateTime Time { get; set; }
    /// <summary>
    /// The user that issued the warning.
    /// </summary>
    ulong IssuerUserId { get; set; }
    /// <summary>
    /// The reason the warning was issued for.
    /// </summary>
    string Reason { get; set; }
    /// <summary>
    /// The number of warnings the user was on after this warning.
    /// </summary>
    ulong Number { get; set; }
    /// <summary>
    /// The type of the warning.
    /// </summary>
    WarningType Type { get;set; }
}
