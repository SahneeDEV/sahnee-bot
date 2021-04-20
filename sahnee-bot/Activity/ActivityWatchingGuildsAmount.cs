using Discord;

namespace sahnee_bot.Activity
{
    public class ActivityWatchingGuildsAmount : IActivity
    {
        public string Name { get; }
        public ActivityType Type { get; }
        public ActivityProperties Flags { get; }
        public string Details { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="amount">the amount of guilds the bot currently is in</param>
        public ActivityWatchingGuildsAmount(int amount)
        {
            this.Name = $"on {amount} guilds";
            this.Type = ActivityType.Watching;
            this.Flags = ActivityProperties.None;
        }
    }
}
