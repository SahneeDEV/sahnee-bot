using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Embeds;
using sahnee_bot.Logging;

namespace sahnee_bot.commands.CommandActions
{
    public class HelpAction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        
        /// <summary>
        /// Executes the help dialog showing
        /// </summary>
        /// <param name="channel">the channel to post to</param>
        /// <returns></returns>
        public async Task HelpActionAsync(ISocketMessageChannel channel)
        {
            try
            {
                //get the embed
                HelpEmbed embed = new HelpEmbed();
                
                //send a message with all the embeds to the channel
                List<EmbedBuilder> allEmbeds = embed.HelpEmbedBuilder();
                for (int i = 0; i < allEmbeds.Count; i++)
                {
                    await channel.SendMessageAsync(null, false, allEmbeds[i].Build());
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "HelpAction:HelpoActionAsync");
            }
        }
    }
}
