using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.Logging;

namespace sahnee_bot.commands.CommandActions
{
    public class UnwarnRoleAction
    {
        //Variables
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Unwarns all users within a specific role
        /// </summary>
        /// <param name="role">the role that users will be unwarned </param>
        /// <param name="guild">the current guild</param>
        /// <param name="channel">the current channel</param>
        /// <param name="message">the message</param>
        /// <param name="reason">the reason if available</param>
        public async Task UnwarnRoleAsync(IRole role,IGuild guild, ISocketMessageChannel channel, IUserMessage message, [Remainder] string reason = "")
        {
            try
            {
                int usersUnwarned = 0;
                //iterate through every user within the role
                foreach (IGuildUser user in await guild.GetUsersAsync())
                {
                    //check if the user is in the specified role
                    foreach (ulong userRoleId in user.RoleIds)
                    {
                        if (userRoleId == role.Id)
                        {
                            //user will be warned
                            UnwarnAction unwarnAction = new UnwarnAction();
                            await unwarnAction.UnwarnAsync(user, guild, channel, message, reason);
;                           usersUnwarned++;
                        }
                    }
                }
                
                //check if any user has been unwarned, if not give the user at least a bit of result
                if (usersUnwarned == 0)
                {
                    await channel.SendMessageAsync("😕 no users were found with that role. Could not unwarn anyone... 💔");
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"Error while unwarning all users from the role: {role.Name}.");
                await _logger.Log(e.Message, LogLevel.Error, "UnwarnRoleAction:UnwarnRoleAsync");
            }
        }
    }
}
