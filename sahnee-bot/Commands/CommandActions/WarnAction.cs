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
    
    public class WarnAction
    {
        //Variables
        private readonly Logging _logging = new Logging();
        private readonly RoleInformation _roleInformation = new RoleInformation();
        private readonly RoleUserInteraction _roleUserInteraction = new RoleUserInteraction();
        private readonly RoleCreation _roleCreation = new RoleCreation();
        private ulong _fromId = 0;
        
        /// <summary>
        /// Executes the warning procedure
        /// </summary>
        /// <param name="user">The IUser Object</param>
        /// <param name="reason">The reason to the user</param>
        /// <param name="guild">the current guild</param>
        /// <param name="channel">the channel to post to</param>
        /// <param name="message">the message itself</param>
        /// <param name="botId">id of the bot, if the user should be warned by the mighty bot himself</param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public async Task WarnAsync(IGuildUser user, string reason, IGuild guild, ISocketMessageChannel channel, IUserMessage message, ulong? botId = 0)
        {
            //Check if the was given a reason for this warning
            //Used for generating the random reason
            bool reasonGiven = reason != null;
            //Variables
            uint userNewWarnings = 0;
            //Role procedure
            try
            {
                //internal Variables
                uint userCurrentWarnings = 0;
                //Get the current warnings of the user
                
                userCurrentWarnings = await _roleInformation.HighestWarningRoleNumberOfUserAsync(user, guild);
                //Increment warnings by one
                userNewWarnings = userCurrentWarnings + 1;
                //create the new warning role name
                string newRoleName = StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix + userNewWarnings;
                //Check if the user has an old role to remove
                if (userCurrentWarnings > 0)
                {
                    //get the current warning role the user is in
                    IRole oldRole = await _roleInformation.HighestWarningRoleRoleUserAsync(user, guild);
                    //remove the user from his previous(his old) warning role
                    if (!await _roleUserInteraction.RemoveUserFromRole(user, oldRole, channel))
                    {
                        throw new Exception();
                    }
                }
                //create the new warning role if it doesnt already exist
                IRole newRole = await _roleCreation.CreateRoleAsync(guild, newRoleName);
                //add the user to the next higher(his new) warning role
                await _roleUserInteraction.AddUserToRoleAsync(user, newRole, channel);
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"😭 I was unable to assign the updated roles! {e.Message}");
                await _logging.LogToConsoleBase(e.Message);
                return;
            }
            //Random Reason generation
            if (!reasonGiven)
            {
                try
                {
                    //TODO: Random reason generating
                }
                catch (Exception e)
                {
                    await channel.SendMessageAsync($"😭 I was unable to create a random reason! {e.Message}");
                    await _logging.LogToConsoleBase(e.Message);
                    return;
                }
            }
            //Send the warning to the database for a histroy
            try
            {
                //Set the from user id
                
                if (botId != null)
                {
                    if (botId != 0)
                    {
                        _fromId = (ulong)botId;
                    }
                    else
                    {
                        _fromId = message.Author.Id;
                    }
                }
                else
                {
                    _fromId = message.Author.Id;
                }
                
                //increment the id
                StaticDatabase.UpdateWarningCollectionId();
                StaticDatabase.UpdateWarningCurrentStateId();
                //create a new schema instance
                WarningBotSchema warningBotSchema = new WarningBotSchema
                {
                    From = _fromId, To = user.Id, Time = DateTime.Now, Reason = reason, WarningType = WarningType.Warning, _id = StaticDatabase.GetWarningCollectionId(), Number = userNewWarnings, GuildId = guild.Id
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
            if (!user.IsBot | botId != 0)
            {
                //Send the reason to the user
                try
                {
                    await user.SendMessageAsync($"👎 [{guild.Name}] You have been warned by <@{_fromId}> in channel {channel.Name}. This is warning #{userNewWarnings}. (Reason: {reason})");
                }
                catch (Exception e)
                {
                    await channel.SendMessageAsync($"<@{message.Author.Id}>, 😭 I was unable to send a private reason to <@{user.Id}>! Cannot send messages to this user.");
                    await _logging.LogToConsoleBase(e.Message);
                }                
            }
            //Send the feedback reason back to the channel
            try
            {
                await channel.SendMessageAsync($"<@{user.Id}> has been warned by <@{_fromId}>. This is warning #{userNewWarnings}. (Reason: {reason})");
            }
            catch (Exception e)
            {
                await _logging.LogToConsoleBase(e.Message);
            }
        }
    }
}
