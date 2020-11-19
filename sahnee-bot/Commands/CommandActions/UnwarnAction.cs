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
    public class UnwarnAction
    {
        //Variables
        private readonly Logging _logging = new Logging();
        private readonly RoleInformation _roleInformation = new RoleInformation();
        private readonly RoleUserInteraction _roleUserInteraction = new RoleUserInteraction();
        private readonly RoleCreation _roleCreation = new RoleCreation();

        /// <summary>
        /// Executes the unwarning procedure
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="reason">the unwarning reason !optional</param>
        /// <param name="guild"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task UnwarnAsync(IGuildUser user,IGuild guild, ISocketMessageChannel channel, IUserMessage message, [Remainder] string reason = "")
        {
            //Variables
            uint userNewWarnings = 0;
            //Role Procedure
            try
            {
                //internal Variables
                uint userCurrentWarnings = 0;
                //Get the current warnings of the user
                userCurrentWarnings = await _roleInformation.HighestWarningRoleNumberOfUserAsync(user, guild);
                //prevent that the user can have negative warnings
                if (userCurrentWarnings == 0)
                {
                    await channel.SendMessageAsync($"<@{user.Id}> has no warnings left. He's freeeeeeeeee.");
                    return;
                }
                //remove one warning
                userNewWarnings = userCurrentWarnings - 1;
                //set the new warning role name
                string newRoleName = StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix + userNewWarnings;
                //get the current warning role the user is in
                IRole oldRole = await _roleInformation.HighestWarningRoleRoleUserAsync(user, guild);
                //create the new warning role if it doesnt already exist
                IRole newRole = await _roleCreation.CreateRoleAsync(guild, newRoleName);
                //remove the user from his previous(his old) warning role
                await _roleUserInteraction.RemoveUserFromRole(user, oldRole, channel);
                //add the user to the next lower(his new) warning role
                if (userNewWarnings > 0)
                {
                    await _roleUserInteraction.AddUserToRoleAsync(user, newRole, channel);
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"😭 I was unable to assign the updated roles! {e.Message}");
                await _logging.LogToConsoleBase(e.Message);
                return;
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
                    currentWarning.Number = currentWarning.Number - 1;
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
                    if (reason != "")
                    { 
                        await user.SendMessageAsync($"❤ [{guild.Name}] One of your warnings has been revoked by <@{message.Author.Id}> in channel {channel.Name}. Only #{userNewWarnings} warnings left!. (Reason: {reason})");
                    }
                    else
                    {
                        await user.SendMessageAsync($"❤ [{guild.Name}] One of your warnings has been revoked by <@{message.Author.Id}> in channel {channel.Name}. Only #{userNewWarnings} warnings left!.");
                    }
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
                if (reason != "")
                {
                    await channel.SendMessageAsync($"❤ <@{user.Id}> was unwarned by <@{message.Author.Id}>. Still #{userNewWarnings} warnings left. (Reason: {reason})");
                }
                else
                {
                    await channel.SendMessageAsync($"❤ <@{user.Id}> was unwarned by <@{message.Author.Id}>. Still #{userNewWarnings} warnings left.");
                }
            }
            catch (Exception e)
            {
                await _logging.LogToConsoleBase(e.Message);
            }
        }
    }
}
