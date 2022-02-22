namespace SahneeBotModel.Contract;

/// <summary>
/// Guild specific data.
/// </summary>
public interface IGuildState: IGuildSpecific
{
    /// <summary>
    /// The channel the bot has been bound to.
    /// </summary>
    ulong? BoundChannelId { get; set; }
    /// <summary>
    /// If roles will be set on the server for the warning count
    /// </summary>
    public bool SetRoles { get; set; }
    /// <summary>
    /// default color for the roles
    /// </summary>
    public string WarningRoleColor { get; set; }
    /// <summary>
    /// The last changelog version this guild has received.
    /// </summary>
    public Version? LastChangelogVersion { get; set; }
}