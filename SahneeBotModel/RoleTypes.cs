namespace SahneeBotModel;

/// <summary>
/// A role type.
/// </summary>
[Flags]
public enum RoleTypes
{
    /// <summary>
    /// No role.
    /// </summary>
    None = 0,
    /// <summary>
    /// Admin privileges.
    /// </summary>
    Administrator = 1,
    /// <summary>
    /// Moderator privileges.
    /// </summary>
    Moderator = 2
}
