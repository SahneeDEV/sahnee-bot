using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.Database.Standards
{
    public class GetAllRolesFromDatabase
    {
        //Variables
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Queries all roles for a specific guild and from a specific type
        /// </summary>
        /// <param name="guildId">the guild to get the roles from</param>
        /// <param name="roletype">the type of role (mod/admin)</param>
        /// <returns></returns>
        public async Task<List<string>> GetAllRolesFromDatabaseAsync(ulong guildId, RoleTypes roletype)
        {
            try
            {
                //get all roletypes for a guild
                List<WarningBotRolesSchema> wantedRoles = StaticDatabase.WarningRolesCollection()
                    .Query()
                    .Where(guid => guid.GuildId == guildId && guid.RoleType == roletype)
                    .ToList();
                
                //get all names of the selected roles
                List<string> rolesToReturn = new List<string>();

                foreach (WarningBotRolesSchema role in wantedRoles)
                {
                    rolesToReturn.Add(role.RoleName);
                }

                return rolesToReturn;
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, $"GetAllRolesFromDatabase:GetAllRolesFromDatabaseAsync");
                return null;
            }
        }
    }
}
