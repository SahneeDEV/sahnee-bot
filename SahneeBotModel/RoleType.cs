namespace SahneeBotModel;

/// <summary>
/// A role type.
/// </summary>
[Flags]
public enum RoleType
{
    /// <summary>
    /// No role.
    /// </summary>
    None = 0b00,
    /// <summary>
    /// Moderator privileges.
    /// </summary>
    Moderator = 0b10,
    /// <summary>
    /// Admin privileges.
    /// </summary>
    Administrator = 0b01
}
