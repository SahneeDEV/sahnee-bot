using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.RoleSystem
{
    public class RoleUserInteraction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        
        /// <summary>
        /// Adds a user to a role and checks if successful
        /// </summary>
        /// <param name="currentUser">the user that should be modified</param>
        /// <param name="role">the role the user should get</param>
        /// <param name="channel">The current context</param>
        /// <returns></returns>
        /// <exception cref="UserNotDefinedException"></exception>
        public async Task<bool> AddUserToRoleAsync(IGuildUser currentUser, IRole role, ISocketMessageChannel channel)
        {
            try
            {
                //Add the user to the role
                if (currentUser == null)
                {
                    await _logger.Log($"Current User is Null. Cannot add {role.Name}", LogLevel.Error, "RoleUserInteraction:AddUserToRoleAsync");
                    throw new UserNotDefinedException("Current User is null", channel);
                }
                await currentUser.AddRoleAsync(role);
                return true;
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
                return false;
            }
        }
        
        
        /// <summary>
        /// Removes a given user from a role
        /// </summary>
        /// <param name="currentUser">the user that will be removed</param>
        /// <param name="role">the role the user will be removed from</param>
        /// <param name="channel"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<bool> RemoveUserFromRole(IGuildUser currentUser, IRole role, ISocketMessageChannel channel)
        {
            try
            {
                //Remove the user from the role
                if (currentUser == null)
                {
                    await _logger.Log($"Current User is Null. Cannot remove {role.Name}", LogLevel.Error, "RoleUserInteraction:RemoveUserFromRole");
                    throw new UserNotDefinedException("Current User is numm", channel);
                }
                await currentUser.RemoveRoleAsync(role);
                return true;
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
                return false;
            }
        }
    }
}
