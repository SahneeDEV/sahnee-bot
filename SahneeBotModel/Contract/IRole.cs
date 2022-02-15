namespace SahneeBotModel.Contract;

/// <summary>
/// A role for the bot.
/// </summary>
public interface IRole: IGuildSpecific
{
    /// <summary>
    /// The name of the Role
    /// </summary>
    public ulong RoleId { get; set; }
    /// <summary>
    /// The type of the Role
    /// </summary>
    public RoleType RoleType { get; set; }
}