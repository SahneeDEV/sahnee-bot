using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Database.Standards;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.Startup.StartupActions
{
    public class UpdateRoleSystem
    {
        //Variables
        private static readonly Logger _logger = new Logger();
        private static readonly GetAllRolesFromDatabase _getAllRolesFromDatabase = new GetAllRolesFromDatabase();


        public static async Task UpdateRoleSystemAsync(IReadOnlyCollection<IGuild> botGuilds)
        {
            //old roles that will be migrated to the database
            string[] oldConfigRolesAdmin = { "Owner", "Administrator", "Admin", "Server Admin"};
            string[] oldConfigRolesMod = { "Moderator", "Mod", "Mods", "Staff", "Community Manager"};
            try
            {
                //go through every guild
                foreach (IGuild guild in botGuilds)
                {
                    //check if the guild has at least one admin role
                    List<string> availableAdminRoles = await _getAllRolesFromDatabase.GetAllRolesFromDatabaseAsync(
                        guild.Id, RoleTypes.WarningBotAdmin);
                    if (availableAdminRoles.Count > 0)
                    {
                        //do nothing, because we assume the guild already uses the new system
                        await _logger.Log($"Guild already using the new system: {guild.Name}",
                            LogLevel.Debug, "UpdateRoleSystem:UpdateRoleSystemAsync");
                    }
                    else
                    {
                        await _logger.Log($"Guild needs migration: {guild.Name}", 
                            LogLevel.Debug, "UpdateRoleSystem:UpdateRoleSystemAsync");
                        //migration necessary, do migration
                        await MigrateToDatabase(guild, oldConfigRolesAdmin, oldConfigRolesMod);
                    }
                    
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message,LogLevel.Error, "UpdateRoleSystem:UpdateRoleSystemAsync");
            }
        }

        /// <summary>
        /// Processes the migration
        /// </summary>
        /// <param name="guild">the guild it's all about</param>
        /// <param name="adminRoles">old config admin roles</param>
        /// <param name="modRoles">old config mod roles</param>
        /// <returns></returns>
        private static async Task MigrateToDatabase(IGuild guild, string[] adminRoles, string[] modRoles)
        {
            try
            {
                AddRoleToDatabase addRoleToDatabase = new AddRoleToDatabase();
                
                //check if any of the roles exist on the guild
                List<IRole> availableRolesOnGuild = new List<IRole>();
                
                //admin roles
                //clear the list
                availableRolesOnGuild.Clear();
                foreach (string adminRole in adminRoles)
                {
                    foreach (IRole role in guild.Roles)
                    {
                        //compare against all roles 
                        if (role.Name == adminRole)
                        {
                            availableRolesOnGuild.Add(role);
                        }
                    }
                }
                
                //add all found admin roles
                foreach (IRole role in availableRolesOnGuild)
                {
                    await addRoleToDatabase.AddRoleToDatabaseAsync(guild.Id, role, RoleTypes.WarningBotAdmin);
                }
                
                //mod roles
                //clear the list
                availableRolesOnGuild.Clear();
                foreach (string modRole in modRoles)
                {
                    foreach (IRole role in guild.Roles)
                    {
                        //compare against all roles 
                        if (role.Name.ToLower() == modRole)
                        {
                            availableRolesOnGuild.Add(role);
                        }
                    }
                }
                
                //add all found mod roles
                foreach (IRole role in availableRolesOnGuild)
                {
                    await addRoleToDatabase.AddRoleToDatabaseAsync(guild.Id, role, RoleTypes.WarningBotMod);
                }
                await _logger.Log($"Migration for guild {guild.Name} done.",LogLevel.Error, "UpdateRoleSystem:MigrateToDatabase");
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message,LogLevel.Error, "UpdateRoleSystem:MigrateToDatabase");
            }
        }
    }
}