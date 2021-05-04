using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Configuration;
using sahnee_bot.Logging;

namespace sahnee_bot.Util
{
    public class GetBotCommandsChannel
    {
        //Variables
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Gets the bot-commands channel of the given guild
        /// </summary>
        /// <param name="guild">the guild to get the channel from</param>
        /// <returns></returns>
        public async Task<IGuildChannel> GetBotCommandsChannelAsync(IGuild guild)
        {
            try
            {
                //get all guild channels and filter the bot-commands channel out of them
                IGuildChannel botCommandsChannel = null!;
                IReadOnlyCollection<IGuildChannel> guildChannels = await guild.GetChannelsAsync(CacheMode.AllowDownload);
                foreach (IGuildChannel guildChannel in guildChannels)
                {
                    if (guildChannel.Name == StaticConfiguration.GetConfiguration().General.CommandChannel)
                    {
                        botCommandsChannel = guildChannel;
                    }
                }
                return botCommandsChannel;
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "GetBotCommandsChannel:GetBotCommandsChannelAsync");
                return null;
            }
        }
    }
}
