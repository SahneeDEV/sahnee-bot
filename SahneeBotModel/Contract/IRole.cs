namespace SahneeBotModel.Contract;

/// <summary>
/// A role for the bot.
/// </summary>
public interface IRole: ISnowflake, IGuildSpecific
{
    /// <summary>
    /// The name of the Role
    /// </summary>
    public string RoleName { get; set; }
    /// <summary>
    /// The type of the Role
    /// </summary>
    public RoleTypes RoleType { get; set; }
}