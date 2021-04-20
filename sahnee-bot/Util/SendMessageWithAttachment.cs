using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database;
using sahnee_bot.Logging;

namespace sahnee_bot.Util
{
    public class SendMessageWithAttachment
    {
        //Variables
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Sends the message to the user with the given attachment
        /// </summary>
        /// <param name="attachments">the original attachment array from the message</param>
        /// <param name="channel">the channel in where the warning/unwarn has been issued to tell if something went wrong</param>
        /// <param name="message">the warn/unwran message</param>
        /// <param name="user">the user that will be warned</param>
        /// <param name="guild">the guild in where the warning should be issued</param>
        /// <param name="_fromId">the id of the user who issued the warning</param>
        /// <param name="userNewWarnings">amount of the new warnings for the user</param>
        /// <param name="reason">reason, why the user got warned/unwarned</param>
        /// <param name="warningType">the warningtype to differentiate between warning and unwarn</param>
        /// <returns></returns>
        public async Task SendUserMessageWithAttachmentAsync(
            IReadOnlyCollection<IAttachment> attachments,
            ISocketMessageChannel channel,
            IUserMessage message,
            IGuildUser user,
            IGuild guild,
            ulong _fromId,
            uint userNewWarnings,
            string reason,
            WarningType warningType)
        {
            try
            {
                //gather information about the sent image
                string attachmentName = "";
                string attachmentUrl = "";
                attachments.All(attach =>
                {
                    attachmentName = attach.Filename;
                    attachmentUrl = attach.Url;
                    return true;
                });
                //download the image into the memory to upload again
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage responseMessage = await client.GetAsync(attachmentUrl))
                    await using (Stream websiteStream = await responseMessage.Content.ReadAsStreamAsync())
                    {
                        //warning
                        if (warningType == WarningType.Warning)
                        {
                            await user.SendFileAsync(websiteStream, attachmentName, $"👎 [{guild.Name}] You have been warned by <@{_fromId}> in channel {channel.Name}. This is warning #{userNewWarnings}. (Reason: {reason})");
                        }
                        //unwarn
                        if (warningType == WarningType.Unwarn)
                        {
                            if (reason != "")
                            { 
                                await user.SendFileAsync(websiteStream,attachmentName, $"❤ [{guild.Name}] One of your warnings has been revoked by <@{message.Author.Id}> in channel {channel.Name}. Only #{userNewWarnings} warnings left!. (Reason: {reason})");
                            }
                            else
                            {
                                await user.SendFileAsync(websiteStream,attachmentName,$"❤ [{guild.Name}] One of your warnings has been revoked by <@{message.Author.Id}> in channel {channel.Name}. Only #{userNewWarnings} warnings left!.");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"<@{message.Author.Id}>, 😭 I was unable to send a private reason to <@{user.Id}>! Cannot send messages to this user.");
                await _logger.Log(e.Message, LogLevel.Error, "SendMessageWithAttachment:SendUserMessageWithAttachmentAsync");
            }
        }

        
        public async Task SendChannelMessageWithAttachmentAsync(
            IReadOnlyCollection<IAttachment> attachments,
            ISocketMessageChannel channel,
            IUserMessage message,
            IGuildUser user,
            ulong fromId,
            uint userNewWarnings,
            string reason,
            WarningType warningType
            )
        {
            try
            {
                //gather information about the sent image
                string attachmentName = "";
                string attachmentUrl = "";
                attachments.All(attach =>
                {
                    attachmentName = attach.Filename;
                    attachmentUrl = attach.Url;
                    return true;
                });
                //download the image into the memory to upload again
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage responseMessage = await client.GetAsync(attachmentUrl))
                    await using (Stream websiteStream = await responseMessage.Content.ReadAsStreamAsync())
                    {
                        //warning
                        if (warningType == WarningType.Warning)
                        {
                            await channel.SendFileAsync(websiteStream, attachmentName,$"<@{user.Id}> has been warned by <@{fromId}>. This is warning #{userNewWarnings}. (Reason: {reason})");

                        }
                        //unwarn
                        if (warningType == WarningType.Unwarn)
                        {
                            if (reason != "")
                            { 
                                await channel.SendFileAsync(websiteStream,attachmentName,$"❤ <@{user.Id}> was unwarned by <@{message.Author.Id}>. Still #{userNewWarnings} warnings left. (Reason: {reason})");
                            }
                            else
                            {
                                await channel.SendFileAsync(websiteStream,attachmentName,$"❤ <@{user.Id}> was unwarned by <@{message.Author.Id}>. Still #{userNewWarnings} warnings left.");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "SendMessageWithAttachment:SendChannelMessageWithAttachmentAsync");
            }
        }
    }
}
