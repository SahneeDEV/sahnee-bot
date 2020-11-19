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
    public class WarnAllAction
    {
        //Variables
        private readonly Logging _logging = new Logging();
        private readonly RoleInformation _roleInformation = new RoleInformation();
        private readonly RoleUserInteraction _roleUserInteraction = new RoleUserInteraction();
        private readonly RoleCreation _roleCreation = new RoleCreation();

        public async Task WarnAllAsync(string reason, IGuild guild, ISocketMessageChannel channel, IUserMessage message)
        {
            //loop for all guild users
            foreach (IGuildUser user in await guild.GetUsersAsync())
            {
                //Variables
                uint userNewWarnings = 0;
                try
                {
                    //internal Variables
                    uint userCurrentWarnings = 0;
                    //Get the current warnings of the user
                    userCurrentWarnings = await _roleInformation.HighestWarningRoleNumberOfUserAsync(user, guild);
                    //add one warning
                    userNewWarnings = userCurrentWarnings + 1;
                    //set the new warning role name
                    string newRoleName = StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix + userNewWarnings;
                    if (userCurrentWarnings > 0)
                    {
                        //get the current warning role the user is in
                        IRole oldRole = await _roleInformation.HighestWarningRoleRoleUserAsync(user, guild);
                        //remove the user from his previous(his old) warning role
                        await _roleUserInteraction.RemoveUserFromRole(user, oldRole, channel);
                    }
                    //create the new warning role if it doesnt already exist
                    IRole newRoleNew = await _roleCreation.CreateRoleAsync(guild, newRoleName);
                    //add the user to the next higher(his new) warning role
                    await _roleUserInteraction.AddUserToRoleAsync(user, newRoleNew, channel);
                }
                catch (Exception e)
                {
                    await channel.SendMessageAsync($"😭 I was unable to assign the updated roles! {e.Message}");
                    await _logging.LogToConsoleBase(e.Message);
                }
                //Send the warning to the database for a histroy
                try
                {
                    //increment the id
                    StaticDatabase.UpdateWarningCollectionId();
                    StaticDatabase.UpdateWarningCurrentStateId();
                    //create a new schema instance
                    WarningBotSchema warningBotSchema = new WarningBotSchema
                    {
                        From = message.Author.Id, To = user.Id, Time = DateTime.Now, Reason = reason, WarningType = WarningType.Warning, _id = StaticDatabase.GetWarningCollectionId(), Number = userNewWarnings, GuildId = guild.Id
                    };
                    //update the current warning number in the table

                    WarningBotCurrentStatesSchema currentWarning = StaticDatabase.WarningCurrentStateCollection().FindOne(usr => usr.UserId == user.Id && usr.GuildId == guild.Id);
                    //check if a user already exists in the database
                    if (currentWarning == null)
                    {
                        WarningBotCurrentStatesSchema warningBotCurrentStatesSchema = new WarningBotCurrentStatesSchema
                        {
                            _id = StaticDatabase.GetWarningCurrentStateId(), Time = DateTime.Now, Number = userNewWarnings, GuildId = guild.Id, UserId = user.Id
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
                }
                catch (Exception e)
                {
                    await channel.SendMessageAsync($"😭 I was unable to add the warning to the history! {e.Message}");
                    await _logging.LogToConsoleBase(e.Message);
                }
                //Dont send messages to bots
                if (!user.IsBot)
                {
                    //Send the message to the user
                    try
                    {
                        await user.SendMessageAsync($"👎 [{guild.Name}] You have been warned by <@{message.Author.Id}> in channel {channel.Name}. This is warning #{userNewWarnings}. (Reason: {reason})");
                    }
                    catch (Exception e)
                    {
                        await channel.SendMessageAsync($"<@{message.Author.Id}>, 😭 I was unable to send a private message to <@{user.Id}>! Cannot send messages to this user.");
                        await _logging.LogToConsoleBase(e.Message);
                    }                    
                }
                //Send the feedback message back to the channel
                try
                {
                    await channel.SendMessageAsync($"<@{user.Id}> has been warned by <@{message.Author.Id}>. This is warning #{userNewWarnings}. (Reason: {reason})");
                }
                catch (Exception e)
                {
                    await _logging.LogToConsoleBase(e.Message);
                }
            }
        }
    }
}
