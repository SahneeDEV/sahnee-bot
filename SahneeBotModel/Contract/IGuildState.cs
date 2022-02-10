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
}