using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.Activity
{
    public class BotActivity
    {
        //Variables
        private static readonly Logger _logger = new Logger();

        /// <summary>
        /// Default startup Method
        /// </summary>
        /// <returns></returns>
        public static async Task ChangeBotActivity()
        {
            try
            {
                DiscordSocketClient bot = StaticBot.GetBot() as DiscordSocketClient;
                await bot.SetActivityAsync(new ActivityWatchingGuildsAmount(bot.Guilds.Count));
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "BotActivity:ChangeBotActivity");
            }
        }

        /// <summary>
        /// Callable for the joining and leaving guild event
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task ChangeBotActivity(IGuild guild)
        {
            try
            {
                DiscordSocketClient bot = StaticBot.GetBot() as DiscordSocketClient;
                await bot.SetActivityAsync(new ActivityWatchingGuildsAmount(bot.Guilds.Count));
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "BotActivity:ChangeBotActivity");
            }
        }
    }
}
