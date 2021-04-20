using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.Database.Standards
{
    public class AddRoleToDatabase
    {
        //Variables
        private readonly Logger _logger = new Logger();

        
        public async Task<bool> AddRoleToDatabaseAsync(ulong guildId, IRole role, RoleTypes roleType)
        {
            try
            {
                //check if the role already is in the database
                List<WarningBotRolesSchema> roleCheck = StaticDatabase.WarningRolesCollection()
                    .Query()
                    .Where(rid => rid.RoleId == role.Id && rid.GuildId == guildId)
                    .ToList();

                if (roleCheck.Count >= 1)
                {
                    return false;
                }

                //create new guid
                Guid g = Guid.NewGuid();
                //Create a new schema instance
                WarningBotRolesSchema warningBotRolesSchema = new WarningBotRolesSchema
                {
                    _id = g.ToString(), Time = DateTime.Now, GuildId = guildId, RoleType = roleType, RoleId = role.Id, RoleName = role.Name
                };
                //insert into the database
                StaticDatabase.WarningRolesCollection().Upsert(warningBotRolesSchema);
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
