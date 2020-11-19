using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands.CommandActions
{
    public class CleanupWarningRolesAction
    {
        //Variables
        
        public async Task CleanupWarningRoles(SocketCommandContext context)
        {
            //Variables
            RoleInformation roleInformation = new RoleInformation();
            Console.WriteLine("Starting Role deletion on all Guilds");
            await context.Channel.SendMessageAsync("Starting role cleanup...🧹");
            //get all roles that are assigned to guildUsers
            List<IRole> assignedRoles = new List<IRole>();
            foreach (var user in context.Guild.Users)
            {
                IRole tempRole = await roleInformation.HighestWarningRoleRoleUserAsync(user, context.Guild);
                if (tempRole.Name.StartsWith(StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix))
                {
                    if (!assignedRoles.Contains(tempRole))
                    {
                        assignedRoles.Add(tempRole as SocketRole);
                    }
                }
            }
            //get all available warning roles in the current guild
            List<IRole> availableRoles = await roleInformation.GetAllAvailableWarningRolesInGuild(context.Guild);
                
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
                Console.WriteLine($"In Guild: {context.Guild.Name} Deleted Roles: {string.Join(", ",notNeededRoles.Select(r => r.Name))}");
            }
            await context.Channel.SendMessageAsync($"Cleaning up done✨\nAmount of roles deleted: {notNeededRoles.Count}");
        }
    }
}
