using System;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Activity;
using sahnee_bot.JoinEvents.Actions;
using sahnee_bot.Logging;
using sahnee_bot.OtherAPI;
using sahnee_bot.Startup.StartupActions;

namespace sahnee_bot.JoinEvents
{
    public static class JoinProcedure
    {
        //Variables
        private static readonly Logger _logger = new Logger();

        /// <summary>
        /// Contains all methods that need to be executed if the bot joins a guild
        /// </summary>
        /// <param name="guild"></param>
        public static async Task JoinProcedureAsync(IGuild guild)
        {
            try
            {
                //add the joined guild, so they don't get the current changelog on restart
                await BroadcastLatestChangeLog.AddNewlyJoinedGuild(guild);
                
                //create bot-commands channel if it does not already exist
                ITextChannel commandChannel = await CreateBotCommandsChannel.CreateBotCommandsChannelAsync(guild);
                
                //send the startup tutorial
                if (commandChannel != null)
                {
                    await StartupTutorial.StartupTutorialAsync(commandChannel);
                }
                
                //update the bot activity
                await BotActivity.ChangeBotActivity(guild);
                
                //update API's
                await SendApiFeedback.SendApiFeedbackAsync(guild);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "JoinProcedure:JoinProcedureAsync");
            }
        }
    }
}
