using Discord;

namespace SahneeBot.Activity;

public class ActivityWatchingGuildsAmount : IActivity
{
    public string Name { get; }
    public ActivityType Type { get; }
    public ActivityProperties Flags { get; }
    public string? Details => null;
    
    public ActivityWatchingGuildsAmount(int amount)
    {
        Name = $"on {amount} server{(amount == 1 ? "" : "s")}";
        Type = ActivityType.Watching;
        Flags = ActivityProperties.None;
    }
}