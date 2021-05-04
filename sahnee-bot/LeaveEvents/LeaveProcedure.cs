using System;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Activity;
using sahnee_bot.Logging;
using sahnee_bot.OtherAPI;

namespace sahnee_bot.LeaveEvents
{
    public static class LeaveProcedure
    {
        //Variables
        private static readonly Logger _logger = new Logger();

        /// <summary>
        /// Contains all methods that need to be executed if the bot leaves a guild
        /// </summary>
        /// <param name="guild">the guild it's all about</param>
        public static async Task LeaveProcedureAsync(IGuild guild)
        {
            try
            {
                //update the activity
                await BotActivity.ChangeBotActivity(guild);
                
                //update the api's
                await SendApiFeedback.SendApiFeedbackAsync(guild);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "LeaveProcedure:LeaveProcedureAsync");
            }
        }
    }
}
