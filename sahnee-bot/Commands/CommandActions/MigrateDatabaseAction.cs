using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands.CommandActions
{
    public class MigrateDatabaseAction
    {
        //Variables
        private readonly Logging _logging = new Logging();
        private readonly RoleInformation _roleInformation = new RoleInformation();
        
        public async Task MigrateDatabaseAsync(SocketCommandContext context)
        {
            try
            {
                //check if the currentWarningTable contains values
                int countState = StaticDatabase.WarningCurrentStateCollection().Count();
                int countWarnings = StaticDatabase.WarningCollection().Count();
                if (countState == 0 && countWarnings > 0)
                {
                    await context.Channel.SendMessageAsync("Starting \"migration\"");
                    //warning count states do not match the warning entries
                    //get the current highest warning role for every user, if 0 or null skipping
                    foreach (IGuildUser user in context.Guild.Users)
                    {
                        uint highestRole = await _roleInformation.FallbackUserCurrentlyHighestNumberOfUser(user, context.Guild);
                        try
                        {
                            StaticDatabase.UpdateWarningCurrentStateId();
                            WarningBotCurrentStatesSchema warningBotCurrentStatesSchema = new WarningBotCurrentStatesSchema
                            {
                                _id = StaticDatabase.GetWarningCurrentStateId(), Time = DateTime.Now, Number = highestRole, GuildId = context.Guild.Id, UserId = user.Id
                            };
                            StaticDatabase.WarningCurrentStateCollection().Insert(warningBotCurrentStatesSchema);
                        }
                        catch (Exception e)
                        {
                            await context.Channel.SendMessageAsync($"😭 I was unable to add the warning for the user {user.Username} to the history! {e.Message}");
                            await _logging.LogToConsoleBase(e.Message);
                        }
                    }
                    //User Feedback
                    await context.Channel.SendMessageAsync("Users have been migrated!");
                }
                else
                {
                    await context.Channel.SendMessageAsync("No \"migration\" because there are already warnings in the other table");
                }
            }
            catch (Exception e)
            {
                await context.Channel.SendMessageAsync($"😭 I was unable to do anything... {e.Message}");
                await _logging.LogToConsoleBase(e.Message);
            }
            
        }
    }
}
