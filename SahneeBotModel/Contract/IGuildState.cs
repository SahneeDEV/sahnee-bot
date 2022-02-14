namespace SahneeBotModel.Contract;

/// <summary>
/// Guild specific data.
/// </summary>
public interface IGuildState: IGuildSpecific
{
    /// <summary>
    /// The channel the bot is bound to.
    /// </summary>
    public string? BoundChannel { get; set; }

    /// <summary>
    /// If roles will be set on the server for the warning count
    /// </summary>
    public bool SetRoles { get; set; }
}