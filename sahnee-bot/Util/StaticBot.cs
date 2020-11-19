using Discord;

namespace sahnee_bot.Util
{
    public static class StaticBot
    {
        private static IDiscordClient sahneeBot;

        /// <summary>
        /// Sets the current bot
        /// </summary>
        /// <param name="bot"></param>
        public static void SetBot(IDiscordClient bot)
        {
            sahneeBot = bot;
        }

        /// <summary>
        /// Returns the current bot
        /// </summary>
        /// <returns></returns>
        public static IDiscordClient GetBot()
        {
            return sahneeBot;
        }
    }
}
