using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using sahnee_bot.Activity;
using sahnee_bot.Logging;
using sahnee_bot.OtherAPI;
using sahnee_bot.OtherAPI.DiscordBotList;
using sahnee_bot.Startup.StartupActions;

namespace sahnee_bot.Startup
{
    public static class StartupProcedure
    {
        //Variables
        private static readonly Logger _logger = new Logger();
        
        /// <summary>
        /// Contains every other logic for things that need to be run after startup
        /// </summary>
        /// <param name="bot">the current bot</param>
        public static async Task StartupProcedureAsync(DiscordSocketClient bot)
        {
            try
            {
                //Changelog Announce procedure
                await BroadcastLatestChangeLog.BroadcastLatestChangeLogAsync(bot);
                //migrate roles if necessary
                await UpdateRoleSystem.UpdateRoleSystemAsync(bot.Guilds);
                //set the activity
                await BotActivity.ChangeBotActivity();
                
                //Add all available APIs
                SendApiFeedback.AddAvailableApi(new DiscordBotList());
                await SendApiFeedback.SendApiFeedbackAsync(null);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "StartupProcedure:StartupProcedureAsync");
            }
        }
    }
}
