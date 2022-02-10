namespace SahneeBotModel.Contract;

/// <summary>
/// Represents the state of a single user.
/// </summary>
public interface IUserState: IGuildSpecific
{
    /// <summary>
    /// The user ID. Not a snowflake, the actual user ID.
    /// </summary>
    ulong UserId { get; set; }
    /// <summary>
    /// The amount of warnings the user is on.
    /// </summary>
    uint WarningNumber { get; set; }
}
