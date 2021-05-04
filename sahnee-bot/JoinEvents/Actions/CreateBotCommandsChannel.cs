using System;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Configuration;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.JoinEvents.Actions
{
    public static class CreateBotCommandsChannel
    {
        //Variables
        private static readonly Logger _logger = new Logger();

        public static async Task<ITextChannel> CreateBotCommandsChannelAsync(IGuild guild)
        {
            try
            {
                GetBotCommandsChannel getBotCommandsChannel = new GetBotCommandsChannel();
                //check if a channel called bot-commands(or similar name from config) already exists
                IGuildChannel botCommandsChannel = await getBotCommandsChannel.GetBotCommandsChannelAsync(guild);
                if (botCommandsChannel != null)
                {
                    return (ITextChannel)botCommandsChannel;
                }
                //create a new channel called
                botCommandsChannel = await guild.CreateTextChannelAsync(StaticConfiguration.GetConfiguration().General.CommandChannel);
                await _logger.Log($"{StaticConfiguration.GetConfiguration().General.CommandChannel} for guild: {guild.Name} has been crated!", LogLevel.Debug, "CreateBotCommandsChannel:CreateBotCommandsChannelAsync");
                return (ITextChannel)botCommandsChannel;
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message + "\n in Guild: " + guild.Name, LogLevel.Error, "CreateBotCommandsChannel:CreateBotCommandsChannelAsync");
                return null;
            }
        }
    }
}
