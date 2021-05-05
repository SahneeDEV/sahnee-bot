#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Embeds;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.JoinEvents.Actions
{
    public class StartupTutorial
    {
        //Variables
        private static readonly Logger _logger = new Logger();

        /// <summary>
        /// Displays the guild a tutorial with the most basic commands
        /// </summary>
        /// <param name="commandChannel">the channel to send to</param>
        public static async Task StartupTutorialAsync(ITextChannel commandChannel)
        {
            try
            {
                //Wait 500 mills
                Thread.Sleep(500);
                await SendStartupTutorialToGuildAsync(commandChannel);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "StartupTutorial:StartupTutorialAsync");
            }
        }

        /// <summary>
        /// Sends the tutorial embed into the bot-commands channel
        /// </summary>
        /// <param name="commandChannel">the channel to send to</param>
        /// <returns></returns>
        private static async Task SendStartupTutorialToGuildAsync(ITextChannel commandChannel)
        {
            try
            {
                //get the embed
                TutorialEmbed embed = new TutorialEmbed();
                //send the message
                List<EmbedBuilder> embeds = embed.TutorialEmbedBuilder();
                foreach (EmbedBuilder embed1 in embeds)
                {
                    await commandChannel.SendMessageAsync(null, false, embed1.Build());
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message,LogLevel.Error, "StartupTutorial:SendStartupTutorialToGuildAsync");
            }
        }
    }
}
