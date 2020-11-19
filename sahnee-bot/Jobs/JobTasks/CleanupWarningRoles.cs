using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.Jobs.JobTasks
{
    public static class CleanupWarningRoles
    {
        /// <summary>
        /// Cleans all not needed warning roles on a server
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public static async Task CleanupWarningRolesRun(DiscordSocketClient bot)
        {
            //Variables
            RoleInformation roleInformation = new RoleInformation();
            Console.WriteLine("Starting Role deletion on all Guilds");
            //get all guildes
            foreach (var guild in bot.Guilds)
            {
                //get all roles that are assigned to guildUsers
                List<IRole> assignedRoles = new List<IRole>();
                foreach (var user in guild.Users)
                {
                    IRole tempRole = await roleInformation.HighestWarningRoleRoleUserAsync(user, guild);
                    if (tempRole.Name.StartsWith(StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix))
                    {
                        if (!assignedRoles.Contains(tempRole))
                        {
                            assignedRoles.Add(tempRole as SocketRole);
                        }
                    }
                }
                //get all available warning roles in the current guild
                List<IRole> availableRoles = await roleInformation.GetAllAvailableWarningRolesInGuild(guild);
                
                //check if every available role is assigned to a user
                List<IRole> notNeededRoles = new List<IRole>();

                foreach (var currentAvailableRole in availableRoles)
                {
                    if (!assignedRoles.Contains(currentAvailableRole))
                    {
                        notNeededRoles.Add(currentAvailableRole);
                    }
                }
                //Delete all not needed roles
                await Task.WhenAll(notNeededRoles.Select(r => r.DeleteAsync()).ToArray());
                if (notNeededRoles.Count > 0)
                {
                    Console.WriteLine($"In Guild: {guild.Name} Deleted Roles: {string.Join(", ",notNeededRoles.Select(r => r.Name))}");
                }
            }
        }
    }
}