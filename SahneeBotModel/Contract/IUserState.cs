namespace SahneeBotModel.Contract;

/// <summary>
/// Represents the state of a single user.
/// </summary>
public interface IUserState: IUserSpecific
{
    /// <summary>
    /// When was the data last deleted? null if never.
    /// </summary>
    DateTime? LastDataDeletion { get; set; }
}