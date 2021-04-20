using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;

namespace sahnee_bot.Database.Standards
{
    public class AddUnwarnToDatabase
    {
        //Variables
        private readonly Logger _logger = new Logger();
        
        public async Task<bool> AddUnwarnAsync(IUserMessage message, ISocketMessageChannel channel, IUser user, string reason, uint userNewWarnings, ulong guildId, string guildName)
        {
            try
            {
                //create new guid
                Guid g = Guid.NewGuid();
                //create a new schema instance
                WarningBotSchema warningBotSchema = new WarningBotSchema
                {
                    From = message.Author.Id, To = user.Id, Time = DateTime.Now, Reason = reason, WarningType = WarningType.Unwarn, _id = g.ToString(), Number = userNewWarnings, GuildId = guildId
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
                    currentWarning.Number = currentWarning.Number - 1;
                    StaticDatabase.WarningCurrentStateCollection().Update(currentWarning);
                }
                //write back to the database
                StaticDatabase.WarningCollection().Insert(warningBotSchema);
                return true;
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"😭 I was unable to add the warning to the history! {e.Message}");
                await _logger.Log(e.Message, LogLevel.Error, $"AddUnwarnToDatabase:AddUnwarnAsync:{guildName},{guildId}:");
                return false;
            }
        }
    }
}
