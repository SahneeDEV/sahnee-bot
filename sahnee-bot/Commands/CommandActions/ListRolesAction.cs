using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database.Standards;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands.CommandActions
{
    public class ListRolesAction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        private readonly GetAllRolesFromDatabase _getAllRolesFromDatabase = new GetAllRolesFromDatabase();

        /// <summary>
        /// Will print all available requested roles
        /// </summary>
        /// <param name="channel">the channel to post to</param>
        /// <param name="guild">the guild to get the roles from</param>
        /// <param name="roleType">the type of role that is requested</param>
        /// <returns></returns>
        public async Task ListRolesActionAsync(ISocketMessageChannel channel, IGuild guild, RoleTypes roleType)
        {
            try
            {
                //get all roles from the requested type
                List<string> availableRoles = await _getAllRolesFromDatabase.GetAllRolesFromDatabaseAsync(
                    guild.Id, roleType);
                
                //generate the message
                string availableRolesString = "";

                for (int i = 0; i < availableRoles.Count; i++)
                {
                    //prevent the separator at the end or beginning
                    availableRolesString += availableRoles[i];

                    if (i <= availableRoles.Count - 2)
                    {
                        availableRolesString += ", ";
                    }
                }

                await channel.SendMessageAsync(
                    $"🙂 These role(s) were found in the database: {availableRolesString}");
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "ListRolesAction:ListRolesActionAsync");
                await channel.SendMessageAsync(
                    $"😕 Unfortunately I was not able to get any role from the database.");
            }
        }
    }
}
