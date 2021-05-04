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
    public class WarnerHistoryAction
    {
        //Variables
        private readonly Logger _logger = new Logger();

        
        public async Task WarnerHistoryActionAsync(IGuildUser user, IGuild guild, ISocketMessageChannel channel, uint? amount = null)
        {
            try
            {
                //get the amount
                amount ??= StaticConfiguration.GetConfiguration().WarningBot.WarningHistoryCount;
                List<WarningBotSchema> userWarnings = new List<WarningBotSchema>();
                userWarnings = StaticDatabase.WarningCollection()
                    .Query()
                    .Where(from => from.From == user.Id && from.GuildId == guild.Id)
                    .Limit((int)amount)
                    .ToList();
                
                //generate the message
                List<string> messages = new List<string>();
                string messageHeader = $"📚 All these warnings have been issued by <@{user.Id}>: 📚\n";
                messages.Add(messageHeader);
                string msg = "";
                string tempMessageBuilder = "";
                foreach (WarningBotSchema userWarning in userWarnings)
                {
                    if (userWarning.WarningType == WarningType.Warning)
                    {
                        msg += $"👎 (Warning: {userWarning.Number}) <@{userWarning.To}> has been warned by <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n";
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
                        msg += $"❤ (Unwarn: {userWarning.Number}) <@{userWarning.To}> has been unwarned by <@{userWarning.From}> at {userWarning.Time} - {userWarning.Reason}.\n";
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
                    await _logger.Log(e.Message, LogLevel.Error,"WarnerHistoryAction:WarnerHistoryAsync:SendMessages");
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error,"WarnerHistoryAction:WarnerHistoryAsync");
            }
        }
    }
}
