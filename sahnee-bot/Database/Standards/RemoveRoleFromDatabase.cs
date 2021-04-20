using System;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.Database.Standards
{
    public class RemoveRoleFromDatabase
    {
        //Variables
        private readonly Logger _logger = new Logger();

        public async Task<bool> RemoveRoleFromDatabaseAsync(ulong guildId, IRole role, RoleTypes roleType)
        {
            try
            {
                //anti-lockout
                //check that there is at least one admin user

                //get the amount of current admin roles in the database for the guild
                int count = StaticDatabase.WarningRolesCollection()
                    .Query()
                    .Where(rid => rid.RoleType == RoleTypes.WarningBotAdmin && rid.GuildId == guildId)
                    .Count();
                
                //check that there are enough roles
                if (count < 2)
                {
                    return false;
                }
                
                //get the entry from the database
                WarningBotRolesSchema warningBotRolesSchema = StaticDatabase.WarningRolesCollection()
                    .Query()
                    .Where(rid => rid.RoleId == role.Id && rid.GuildId == guildId)
                    .First();

                //Check if the entry exists
                if (warningBotRolesSchema == null)
                {
                    return false;
                }
                
                //remove from the database
                return StaticDatabase.WarningRolesCollection().Delete(warningBotRolesSchema._id);

            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
                return false;
            }
        }
    }
}