using System;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Configuration;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.Startup
{
    public class CreateBotCommandsChannel
    {
        //Variables
        private static readonly Logger _logger = new Logger();

        
        public static async Task CreateBotCommandsChannelAsync(IGuild guild)
        {
            try
            {
                //check if a channel called bot-commands(or similar name from config) already exists
                if (await GetBotCommandsChannel.GetBotCommandsChannelAsync(guild) != null)
                {
                    return;
                }
                //create a new channel called
                await guild.CreateTextChannelAsync(StaticConfiguration.GetConfiguration().General.CommandChannel);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message + "\n in Guild: " + guild.Name, LogLevel.Error, "CreateBotCommandsChannel:CreateBotCommandsChannelAsync");
            }
        }
    }
}
