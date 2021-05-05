using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Logging;

namespace sahnee_bot.commands.CommandActions
{
    public class WarnRoleAction
    {
        //Variables
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// executes the warning of all users that have the role
        /// </summary>
        /// <param name="role">the role that will be warned</param>
        /// <param name="reason">The reason to the users</param>
        /// <param name="guild">the current guild</param>
        /// <param name="channel">the channel to post to</param>
        /// <param name="message">the message itself</param>
        /// <param name="botId">id of the bot, if the user should be warned by the mighty bot himself</param>
        public async Task WarnRoleAsync(IRole role, string reason, IGuild guild, ISocketMessageChannel channel, IUserMessage message, ulong? botId = 0)
        {
            try
            {
                int usersWarned = 0;
                //iterate through every user within the role
                foreach (IGuildUser user in await guild.GetUsersAsync())
                {
                    //check if the user is in the specified role
                    foreach (ulong userRoleId in user.RoleIds)
                    {
                        if (userRoleId == role.Id)
                        {
                            //user will be warned
                            WarnAction warnAction = new WarnAction();
                            await warnAction.WarnAsync(user, reason, guild, channel, message);
                            usersWarned++;
                        }
                    }
                }
                
                //check if any user has been warned, if not give the user at least a bit of result
                if (usersWarned == 0)
                {
                    await channel.SendMessageAsync("😕 no users were found with that role. Could not warn anyone...");
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"Error while warning all users from the role: {role.Name}.");
                await _logger.Log(e.Message, LogLevel.Error, "WarnRoleAction:WarnRoleAsync");
            }
        }
    }
}
