#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Configuration;
using sahnee_bot.Embeds;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.Startup
{
    public class StartupTutorial
    {
        //Variables
        private static readonly Logger _logger = new Logger();

        
        public static async Task StartupTutorialAsync(SocketGuild guild)
        {
            try
            {
                //Wait 5 Seconds
                Thread.Sleep(5000);
                await SendStartupTutorialToGuildAsync(guild);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "StartupTutorial:StartupTutorialAsync");
            }
        }

        /// <summary>
        /// Sends the tutorial embed into the bot-commands channel
        /// </summary>
        /// <param name="guild">the guild that just joined</param>
        /// <returns></returns>
        private static async Task SendStartupTutorialToGuildAsync(IGuild guild)
        {
            try
            {
                //get all guild channels and filter the bot-commands channel out of them
                IGuildChannel botCommandsChannel = await GetBotCommandsChannel.GetBotCommandsChannelAsync(guild);
                //get the embed
                TutorialEmbed embed = new TutorialEmbed();
                //send the message
                ISocketMessageChannel botCommandsMessageChannel = (ISocketMessageChannel)botCommandsChannel;
                List<EmbedBuilder> embeds = embed.TutorialEmbedBuilder();
                foreach (EmbedBuilder embed1 in embeds)
                {
                    await botCommandsMessageChannel.SendMessageAsync(null, false, embed1.Build());
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message,LogLevel.Error, "StartupTutorial:SendStartupTutorialToGuildAsync");
            }
        }
    }
}
