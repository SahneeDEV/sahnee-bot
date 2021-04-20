using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Configuration;
using sahnee_bot.Database;
using sahnee_bot.Database.Standards;
using sahnee_bot.Exceptions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;
using sahnee_bot.UserInformation;
using sahnee_bot.Util;

namespace sahnee_bot.commands.CommandActions
{
    
    public class WarnAction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        private readonly RoleUserInteraction _roleUserInteraction = new RoleUserInteraction();
        private readonly SendMessageWithAttachment _sendMessageWithAttachment = new SendMessageWithAttachment();
        private readonly UserRoles _userRoles = new UserRoles();
        private readonly AddWarningToDatabase _addWarningToDatabase = new AddWarningToDatabase();

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
        public async Task<bool> WarnAsync(IGuildUser user, string reason, IGuild guild, ISocketMessageChannel channel, IUserMessage message, ulong? botId = 0)
        {
            //Check if the was given a reason for this warning
            //Used for generating the random reason
            bool reasonGiven = reason != null;
            //Variables
            uint userNewWarnings = 0;
            ulong fromId = 0;
            //Role procedure
            try
            {
                //internal Variables
                uint userCurrentWarnings = 0;
                //Get the current warnings of the user
                try
                {
                    userCurrentWarnings = await _userRoles.GetUserCurrentWarningNumberDb(user.Id, guild.Id);
                }
                catch (Exception e)
                {
                    if (e is UserNotInDatabaseException)
                    {
                        await _logger.Log($"Could not get current for user {user.Nickname}.\n Hes not in the database yet.", LogLevel.Info);
                    }
                }
                //Set the from user id
                if (botId != null)
                {
                    if (botId != 0)
                    {
                        fromId = (ulong)botId;
                    }
                    else
                    {
                        fromId = message.Author.Id;
                    }
                }
                else
                {
                    fromId = message.Author.Id;
                }
                //Increment warnings by one
                userNewWarnings = userCurrentWarnings + 1;
                //Send the warning to the database for a history
                if (!await _addWarningToDatabase.AddWarningAsync(message, channel, user, reason, userNewWarnings, guild.Id, guild.Name,fromId, botId))
                {
                    throw new CouldNotWriteIntoDatabaseException(user.Nickname);
                }
                //create the new warning role name
                string newRoleName = StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix + userNewWarnings;
                //Check if the user has an old role to remove
                if (userCurrentWarnings > 0)
                {
                    //remove all warning roles from the user
                    await _userRoles.DeleteNotNeededWarningRolesFromUser(user, guild);
                }
                //create the new warning role if it doesnt already exist
                //IRole newRole = await _roleCreation.CreateRoleAsync(guild, newRoleName);
                IRole newRole = await _userRoles.CreateRoleAsync(guild, newRoleName);
                //add the user to the next higher(his new) warning role
                await _roleUserInteraction.AddUserToRoleAsync(user, newRole, channel);
            }
            catch (Exception e)
            {
                if (e is CouldNotWriteIntoDatabaseException)
                {
                    await channel.SendMessageAsync("😭 I was unable to update the warning in the database");
                    await _logger.Log(e.Message, LogLevel.Error,"WarnAction:WarnAsync");
                    return false;
                }
                await channel.SendMessageAsync($"😭 I was unable to assign the updated roles! {e.Message}");
                await _logger.Log(e.Message, LogLevel.Error,"WarnAction:WarnAsync");
                return false;
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
                    await _logger.Log(e.Message, LogLevel.Error,"WarnAction:WarnAsync");
                    return false;
                }
            }
            //Dont send messages to bots
            if (!user.IsBot | botId != 0)
            {
                //Send the reason to the user
                try
                {
                    //Check if an Image/Discord CDN Attachment/any url source was in the message, if so, print at the end
                    if (reason != null && reason.Contains("https://"))
                    {
                        int reasonStart = reason.IndexOf("https://",StringComparison.Ordinal);
                        string cdnContent = reason.Substring(reasonStart ,reason.Length - reasonStart);
                        string reasonWithoutCdnContent = reason.Remove(reasonStart - 1);
                        await user.SendMessageAsync($"<@{user.Id}> has been warned by <@{fromId}>in channel {channel.Name}. This is warning #{userNewWarnings}. (Reason: {reasonWithoutCdnContent})\n{cdnContent}");
                    }
                    else if (message.Attachments.Count > 0)
                    {
                        await _sendMessageWithAttachment.SendUserMessageWithAttachmentAsync(
                            message.Attachments,
                            channel,
                            message,
                            user,
                            guild,
                            fromId,
                            userNewWarnings,
                            reason,
                            WarningType.Warning
                            );
                    }
                    else
                    {
                        await user.SendMessageAsync($"👎 [{guild.Name}] You have been warned by <@{fromId}> in channel {channel.Name}. This is warning #{userNewWarnings}. (Reason: {reason})");
                    }
                }
                catch (Exception e)
                {
                    await channel.SendMessageAsync($"<@{message.Author.Id}>, 😭 I was unable to send a private reason to <@{user.Id}>! Cannot send messages to this user.");
                    await _logger.Log(e.Message, LogLevel.Error, "WarnAction:WarnAsync");
                }
            }
            //Send the feedback reason back to the channel
            try
            {
                //Check if an Image/Discord CDN Attachment/any url source was in the message, if so, print at the end
                if (reason != null && reason.Contains("https://"))
                {
                    int reasonStart = reason.IndexOf("https://",StringComparison.Ordinal);
                    string cdnContent = reason.Substring(reasonStart ,reason.Length - reasonStart);
                    string reasonWithoutCdnContent = reason.Remove(reasonStart - 1);
                    await channel.SendMessageAsync($"<@{user.Id}> has been warned by <@{fromId}>. This is warning #{userNewWarnings}. (Reason: {reasonWithoutCdnContent})\n{cdnContent}");
                }
                else if (message.Attachments.Count > 0)
                {
                    await _sendMessageWithAttachment.SendChannelMessageWithAttachmentAsync(
                        message.Attachments,
                        channel,
                        message,
                        user,
                        fromId,
                        userNewWarnings,
                        reason,
                        WarningType.Warning
                    );
                }
                else
                {
                    await channel.SendMessageAsync($"<@{user.Id}> has been warned by <@{fromId}>. This is warning #{userNewWarnings}. (Reason: {reason})");
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "WarnAction:WarnAsync");
            }

            return true;
        }
    }
}
