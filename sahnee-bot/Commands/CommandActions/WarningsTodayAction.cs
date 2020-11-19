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
        /// Executes the message creating and information gathering process
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task WarningsTodayAsync(IGuildUser user, IGuild guild, ISocketMessageChannel channel)
        {
            var userWarningBotSchemata = StaticDatabase.WarningCollection().Query()
                .Where(to => to.To == user.Id && to.GuildId == guild.Id && to.Time.Date == DateTime.Today)
                .OrderByDescending(date => date.Time)
                .ToList();
            //generate the message
            List<string> messages = new List<string>();
            string messageHeader = $"📚 This is the warning history for <@{user.Id}> 📚 for this date: {DateTime.Now:dd.MM.yyy}\n";
            messages.Add(messageHeader);
            string tempMessageBuilder = "";
            string msg = "";
            foreach (WarningBotSchema userWarning in userWarningBotSchemata)
            {
                if (userWarning.WarningType == WarningType.Warning)
                {
                    msg += $"👎 (Warning: {userWarning.Number}) Warned from <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n";
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
                    msg += $"❤ (Warning: {userWarning.Number}) Unwarned from <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n";
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
