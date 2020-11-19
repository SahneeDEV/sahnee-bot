using System;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace sahnee_bot.RoleSystem
{
    public class RoleCreation
    {
        /// <summary>
        /// Creates a new role on the server in the current guild
        /// </summary>
        /// <param name="currentGuild">the current guild</param>
        /// <param name="currentRoleName">how the role should be named</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IRole> CreateRoleAsync(IGuild currentGuild, string currentRoleName)
        {
            //Check if the role already exists
            foreach (IRole role in currentGuild.Roles)
            {
                if (role.Name == currentRoleName)
                {
                    return role;
                }
            }
            //Create the new role if the role does not exist already
            IRole newRole = await currentGuild.CreateRoleAsync(currentRoleName, default, Color.DarkGrey, false, null);
            return newRole;
        }
    }
}
