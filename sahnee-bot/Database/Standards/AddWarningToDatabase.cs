using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;

namespace sahnee_bot.Database.Standards
{
    public class AddWarningToDatabase
    {
        //Variables
        private readonly Logger _logger = new Logger();
        
        /// <summary>
        /// Updates the users warning in the database
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="channel">the channel to send errors to</param>
        /// <param name="user">the warned user</param>
        /// <param name="reason">reason</param>
        /// <param name="userNewWarnings">amount of new warnings</param>
        /// <param name="guildId">id of the current guild</param>
        /// <param name="guildName">name of the current guild</param>
        /// <param name="fromId">the id from the user that has warned</param>
        /// <param name="botId">id of the bot, if warned by bot</param>
        /// <returns></returns>
        public async Task<bool> AddWarningAsync(IUserMessage message, ISocketMessageChannel channel, IUser user, string reason, uint userNewWarnings, ulong guildId, string guildName, ulong fromId, ulong? botId)
        {
            try
            {
                //new guid
                Guid g = Guid.NewGuid();
                //create a new schema instance
                WarningBotSchema warningBotSchema = new WarningBotSchema
                {
                    From = fromId, To = user.Id, Time = DateTime.Now, Reason = reason, WarningType = WarningType.Warning, _id = g.ToString(), Number = userNewWarnings, GuildId = guildId
                };
                //update the current warning number in the table

                WarningBotCurrentStatesSchema currentWarning = StaticDatabase.WarningCurrentStateCollection().FindOne(usr => usr.UserId == user.Id && usr.GuildId == guildId);
                //check if a user already exists in the database
                if (currentWarning == null)
                {
                    WarningBotCurrentStatesSchema warningBotCurrentStatesSchema = new WarningBotCurrentStatesSchema
                    {
                        _id = g.ToString(), Time = DateTime.Now, Number = userNewWarnings, GuildId = guildId, UserId = user.Id
                    };
                    StaticDatabase.WarningCurrentStateCollection().Insert(warningBotCurrentStatesSchema);
                }
                else
                {
                    currentWarning.Number = currentWarning.Number + 1;
                    StaticDatabase.WarningCurrentStateCollection().Update(currentWarning);
                }
                //write back to the database
                StaticDatabase.WarningCollection().Insert(warningBotSchema);
                return true;
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"😭 I was unable to add the warning to the history! {e.Message}");
                await _logger.Log(e.Message, LogLevel.Error, $"AddWarningToDatabase:AddWarningAsync:{guildName},{guildId}:");
                return false;
            }
        }
    }
}
