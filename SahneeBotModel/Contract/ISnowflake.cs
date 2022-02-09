namespace SahneeBotModel.Contract;

/// <summary>
/// An object with a Snowflake.
/// </summary>
public interface ISnowflake
{
    /// <summary>
    /// The snowflake.
    /// </summary>
    long Id { get; set; }
}