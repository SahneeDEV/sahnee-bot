namespace SahneeBotController;

/// <summary>
/// A warning for the Sahnee bot.
/// </summary>
public interface IWarning
{
    DateTime Time { get; set; }
    ulong GuildId { get; set; }
    ulong From { get; set; }
    ulong To { get; set; }
    string Reason { get; set; }
    ulong Number { get; set; }
}
