namespace SahneeBotModel.Contract;

/// <summary>
/// Guild specific data.
/// </summary>
public interface IGuildSpecific
{
    /// <summary>
    /// The guild ID.
    /// </summary>
    ulong GuildId { get; set; }
}