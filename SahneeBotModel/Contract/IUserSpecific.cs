namespace SahneeBotModel.Contract;

/// <summary>
/// User specific data.
/// </summary>
public interface IUserSpecific
{
    /// <summary>
    /// The user ID. Not a snowflake, the actual user ID.
    /// </summary>
    ulong UserId { get; set; }
}