using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.Configuration;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Database.Standards;
using sahnee_bot.Exceptions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;
using sahnee_bot.UserInformation;
using sahnee_bot.Util;

namespace sahnee_bot.commands.CommandActions
{
    public class UnwarnAction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        private readonly RoleUserInteraction _roleUserInteraction = new RoleUserInteraction();
        private readonly SendMessageWithAttachment _sendMessageWithAttachment = new SendMessageWithAttachment();
        private readonly UserRoles _userRoles = new UserRoles();
        private readonly AddUnwarnToDatabase _addUnwarnToDatabase = new AddUnwarnToDatabase();

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
                //prevent that the user can have negative warnings
                if (userCurrentWarnings == 0)
                {
                    await channel.SendMessageAsync($"<@{user.Id}> has no warnings left. He's freeeeeeeeee🥰.");
                    return;
                }
                //remove one warning
                userNewWarnings = userCurrentWarnings - 1;
                //Send the warning to the database for a histroy
                if (!await _addUnwarnToDatabase.AddUnwarnAsync(message, channel, user, reason, userNewWarnings, guild.Id, guild.Name))
                {
                    throw new CouldNotWriteIntoDatabaseException(user.Nickname);
                }
                //set the new warning role name
                string newRoleName = StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix + userNewWarnings;
                //remove all warning roles from the user
                await _userRoles.DeleteNotNeededWarningRolesFromUser(user, guild);
                //create the new warning role if it doesnt already exist
                IRole newRole = await _userRoles.CreateRoleAsync(guild, newRoleName);
                //add the user to the next lower(his new) warning role
                if (userNewWarnings > 0)
                {
                    await _roleUserInteraction.AddUserToRoleAsync(user, newRole, channel);
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"😭 I was unable to assign the updated roles! {e.Message}");
                await _logger.Log(e.Message, LogLevel.Error, "UnwarnAction:UnwarnAsync");
                return;
            }
            //Dont send messages to bots
            if (!user.IsBot)
            {
                //Send the message to the user
                try
                {
                    //Check if an Image/Discord CDN Attachment/any url source was in the message, if so, print at the end
                    if (reason != null && reason.Contains("https://"))
                    {
                        int reasonStart = reason.IndexOf("https://",StringComparison.Ordinal);
                        string cdnContent = reason.Substring(reasonStart ,reason.Length - reasonStart);
                        string reasonWithoutCdnContent = reason.Remove(reasonStart - 1);
                        await user.SendMessageAsync($"❤ [{guild.Name}] One of your warnings has been revoked by <@{message.Author.Id}> in channel {channel.Name}. Only #{userNewWarnings} warnings left!. (Reason: {reasonWithoutCdnContent})\n{cdnContent}");
                    }
                    else if (message.Attachments.Count > 0)
                    {
                        await _sendMessageWithAttachment.SendUserMessageWithAttachmentAsync(
                            message.Attachments,
                            channel,
                            message,
                            user,
                            guild,
                            message.Author.Id,
                            userNewWarnings,
                            reason,
                            WarningType.Unwarn
                            );
                    }
                    else
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
                    
                }
                catch (Exception e)
                {
                    await channel.SendMessageAsync($"<@{message.Author.Id}>, 😭 I was unable to send a private message to <@{user.Id}>! Cannot send messages to this user.");
                    await _logger.Log(e.Message, LogLevel.Error, "UnwarnAction:UnwarnAsync:");
                }                
            }
            //Send the feedback message back to the channel
            try
            {
                //Check if an Image/Discord CDN Attachment/any url source was in the message, if so, print at the end
                if (reason != null && reason.Contains("https://"))
                {
                    int reasonStart = reason.IndexOf("https://",StringComparison.Ordinal);
                    string cdnContent = reason.Substring(reasonStart ,reason.Length - reasonStart);
                    string reasonWithoutCdnContent = reason.Remove(reasonStart - 1);
                    await channel.SendMessageAsync($"❤ <@{user.Id}> was unwarned by <@{message.Author.Id}>. Still #{userNewWarnings} warnings left. (Reason: {reasonWithoutCdnContent})\n{cdnContent}");
                }
                else if (message.Attachments.Count > 0)
                {
                    await _sendMessageWithAttachment.SendChannelMessageWithAttachmentAsync(
                        message.Attachments,
                        channel,
                        message,
                        user,
                        message.Author.Id,
                        userNewWarnings,
                        reason,
                        WarningType.Unwarn
                    );
                }
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
                await _logger.Log(e.Message, LogLevel.Error, "UnwarnAction:UnwarnAsync:");
            }
        }
    }
}
