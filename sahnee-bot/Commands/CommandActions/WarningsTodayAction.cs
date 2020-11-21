#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Util;

namespace sahnee_bot.commands.CommandActions
{
    public class WarningsTodayAction
    {
        //Variables
        private readonly Logging _logging = new Logging();
        
        /// <summary>
        /// Executes the information gathering process and then starts the message creation and message delivery process
        /// Specific user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task WarningsTodayAsync(IGuildUser user, IGuild guild, ISocketMessageChannel channel)
        {
            IList<WarningBotSchema> userWarningBotSchemata = StaticDatabase.WarningCollection().Query()
                .Where(to => to.To == user.Id && to.GuildId == guild.Id && to.Time.Date == DateTime.Today)
                .OrderByDescending(date => date.Time)
                .ToList();
            await CreateMessagesAndSendAsync(user, userWarningBotSchemata, channel);
        }

        /// <summary>
        /// Executes the information gathering process and then starts the message creation and message delivery process
        /// all users in the guild
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task WarningsTodayAsync(IGuild guild, ISocketMessageChannel channel)
        {
            IList<WarningBotSchema> allUserWarningBotSchemata = StaticDatabase.WarningCollection().Query()
                .Where(g => g.GuildId == guild.Id && g.Time.Date == DateTime.Today)
                .OrderByDescending(date => date.Time)
                .ToList();
            await CreateMessagesAndSendAsync(null, allUserWarningBotSchemata, channel);
        }

        /// <summary>
        /// Creates the message and sends it into the channel
        /// </summary>
        /// <param name="userWarningBotSchemata"></param>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task CreateMessagesAndSendAsync(IGuildUser? user, IList<WarningBotSchema> userWarningBotSchemata, ISocketMessageChannel channel)
        {
            //generate the message
            List<string> messages = new List<string>();
            
            string messageHeader = user != null ? $"📚 This is the warning history for <@{user.Id}> 📚 for this date: {DateTime.Now:dd.MM.yyy}\n" : $"📚 This is the warning history for all users for this date: {DateTime.Now:dd.MM.yyy}\n";
            messages.Add(messageHeader);
            string tempMessageBuilder = "";
            string msg = "";
            foreach (WarningBotSchema userWarning in userWarningBotSchemata)
            {
                if (userWarning.WarningType == WarningType.Warning)
                {
                    msg += user != null ? $"👎 (Warning: {userWarning.Number}) Warned from <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n" 
                        : $"👎 (Warning: {userWarning.Number}) <@{userWarning.To}> warned from <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n";
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
                    if (string.IsNullOrEmpty(userWarning.Reason) || string.IsNullOrWhiteSpace(userWarning.Reason))
                    {
                        msg += user != null ? $"❤ (Unwarn: {userWarning.Number}) Unwarned from <@{userWarning.From}> at {userWarning.Time} - [No Reason given]\n"
                            : $"❤ (Unwarn: {userWarning.Number}) <@{userWarning.To}> unwarned from <@{userWarning.From}> at {userWarning.Time} - [No Reason given]\n";
                    }
                    else
                    {
                        msg += user != null ? $"❤ (Unwarn: {userWarning.Number}) Unwarned from <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n"
                            : $"❤ (Unwarn: {userWarning.Number}) <@{userWarning.To}> unwarned from <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n";
                    }
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
                foreach (string message in messages)
                {
                    await channel.SendMessageAsync(message);
                    Thread.Sleep(200);
                }
            }
            catch (Exception e)
            {
                await _logging.LogToConsoleBase(e.Message);
            }
        }
    }
}
