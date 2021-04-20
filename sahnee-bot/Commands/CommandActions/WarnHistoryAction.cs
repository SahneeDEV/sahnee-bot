using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Configuration;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.commands.CommandActions
{
    public class WarnHistoryAction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        private readonly string[] _allSpecialCommands = new string[1] {"all"};

        /// <summary>
        /// Get the warn history of a specific user with an optional specific amount of items
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <param name="amount"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task WarnHistoryAsync(IGuildUser user, IGuild guild, ISocketMessageChannel channel, IUserMessage message, uint? amount = null)
        {
            try
            {
                await this.ExecuteWarnHistoryAsync(user, amount, guild, SpecialCommands.Custom, channel, message);
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"😭 Something went wrong {e.Message}");
                await _logger.Log(e.Message, LogLevel.Error, "WarnHistoryAction:WarnHistoryAsync");
                return;
            }
        }

        /// <summary>
        /// Get the warn history of a specific user with an optional parameter to do predefined things
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <param name="specialCommand"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task WarnHistoryAsync(IGuildUser user, IGuild guild, ISocketMessageChannel channel, IUserMessage message, string specialCommand)
        {
            try
            {
                //check which special command will be passed
                if (specialCommand == _allSpecialCommands[0])
                {
                    await this.ExecuteWarnHistoryAsync(user, null, guild,SpecialCommands.All, channel, message);
                }
                else
                {
                    await channel.SendMessageAsync(
                                        $"😕 I dont know what you want to tell me with this parameter: {specialCommand}.");
                                    return;
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"😭 Something went wrong {e.Message}");
                await _logger.Log(e.Message, LogLevel.Error, "WarnHistoryAction:WarnHistoryAsync");
                return;
            }
        }
        
        

        /// <summary>
        /// Handles the history generation itself
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="amount">the amount of warnings to show</param>
        /// <param name="specialCommand">the type of special command</param>
        /// <param name="guild">the current context</param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task ExecuteWarnHistoryAsync(IGuildUser user, uint? amount, IGuild guild, SpecialCommands specialCommand, ISocketMessageChannel channel, IUserMessage message)
        {
            amount ??= StaticConfiguration.GetConfiguration().WarningBot.WarningHistoryCount;
            //get the given amount of warnings/unwarnings from the history database table
            List<WarningBotSchema> userWarningBotSchemata = new List<WarningBotSchema>();
            //check if the user is null
            if (user != null)
            {
                if (specialCommand == SpecialCommands.Custom)
                {
                    userWarningBotSchemata = StaticDatabase.WarningCollection().Query()
                        .Where(to => to.To == user.Id && to.GuildId == guild.Id)
                        .OrderByDescending(date => date.Time)
                        .Limit((int)amount)
                        .ToList();
                }
                if (specialCommand == SpecialCommands.All)
                {
                    userWarningBotSchemata = StaticDatabase.WarningCollection().Query()
                        .Where(to => to.To == user.Id && to.GuildId == guild.Id)
                        .OrderByDescending(date => date.Time)
                        .ToList();
                }
            }
            else
            {
                userWarningBotSchemata = StaticDatabase.WarningCollection().Query()
                    .Where(to => to.GuildId == guild.Id)
                    .OrderByDescending(date => date.Time)
                    .Limit((int) amount)
                    .ToList();
            }

            //generate the message
            List<string> messages = new List<string>();
            string messageHeader = "";
            if (user != null)
            {
                messageHeader = $"📚 This is the warning history for <@{user.Id}>: 📚\n";
            }
            else
            {
                messageHeader = $"📚 This is the warning history for everybody: 📚\n";
            }
             
            messages.Add(messageHeader);
            string tempMessageBuilder = "";
            string msg = "";
            foreach (WarningBotSchema userWarning in userWarningBotSchemata)
            {
                if (userWarning.WarningType == WarningType.Warning)
                {
                    msg += user != null ?
                        $"👎 (Warning: {userWarning.Number}) Warned from <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n" :
                        $"👎 (Warning: {userWarning.Number}) <@{userWarning.To}> has been warned by <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n";
                    //check if we exceed the length with this string
                    if (tempMessageBuilder.Length + msg.Length > StaticInternalConfiguration.CharacterLimitMessageOutbound)
                    {
                        //append the string to the list
                        messages.Add(tempMessageBuilder);
                        //clear the StringBuilder
                        tempMessageBuilder = "";
                    }
                    tempMessageBuilder += msg;
                }
                if (userWarning.WarningType == WarningType.Unwarn)
                {
                    msg += user != null ?
                        $"❤ (Unwarn: {userWarning.Number}) Unwarned from <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n" :
                        $"❤ (Unwarn: {userWarning.Number}) <@{userWarning.To}> has been unwarned by <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n";
                    //check if we exceed the length with this string
                    //check if we exceed the length with this string
                    if (tempMessageBuilder.Length + msg.Length > StaticInternalConfiguration.CharacterLimitMessageOutbound)
                    {
                        //append the string to the list
                        messages.Add(tempMessageBuilder);
                        //clear the StringBuilder
                        tempMessageBuilder = "";
                    }
                    tempMessageBuilder += msg;
                }
                //Clear for next run
                msg = "";
            }
            //finally append the StringBuilder to the list
            messages.Add(tempMessageBuilder);
            //Send the result back into the channel
            try
            {
                foreach (string currentMessage in messages)
                {
                    await channel.SendMessageAsync(currentMessage);
                    Thread.Sleep(200);
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error,"WarnHistoryAction:ExecuteWarnHistoryAsync");
            }
        }
        
    }

    /// <summary>
    /// SpecialCommands Enums
    /// </summary>
    enum SpecialCommands
    {
        None = 0,
        All = 1,
        Custom = 2
    }
}
