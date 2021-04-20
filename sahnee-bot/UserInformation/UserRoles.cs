using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Configuration;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Exceptions;
using sahnee_bot.Logging;

namespace sahnee_bot.UserInformation
{
    public class UserRoles
    {
        //Variables
        private readonly Logger _logger = new Logger();
        
        /// <summary>
        /// Gets the currently highest warning role number from the database for a specific user in a specific guild
        /// </summary>
        /// <param name="user">the users id</param>
        /// <param name="guild">the guilds id</param>
        /// <returns></returns>
        public async Task<IRole> GetUserCurrentWarningRoleDb(IGuildUser user, IGuild guild)
        {
            try
            {
                //get the current warning role number of the user
                List<WarningBotCurrentStatesSchema> allEntries = StaticDatabase.WarningCurrentStateCollection().Query()
                    .Where(u => u.UserId == user.Id && u.GuildId == guild.Id)
                    .ToList();
                //now lets get the actual role and return it
                //get all roles the user currently is in
                List<IRole> allRoles = await GetUserRolesAsync(user, guild);
                //get the role that matches
                string roleToFind = StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix +
                                    allEntries[0].Number;
                //get the old role
                IRole oldRole = allRoles.Find(role => role.Name == roleToFind);
                //if user does not have the correct role
                if (oldRole == null)
                {
                    return null;
                }
                return oldRole;
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "UserRoles:GetUserCurrentWarningRoleDb");
                return null;
            }
        }

        /// <summary>
        /// Returns the current warning number for the user from the database
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="guildId">id of the guild</param>
        /// <returns></returns>
        public Task<uint> GetUserCurrentWarningNumberDb(ulong userId, ulong guildId)
        {
            try
            {
                //get the amount
                return Task.FromResult(StaticDatabase.WarningCurrentStateCollection().Query()
                       .Where(u => u.UserId == userId && u.GuildId == guildId)
                       .First()
                       .Number
                );
            }
            catch (Exception e)
            {
                throw new UserNotInDatabaseException(e.Message);
            }
        }
        
        
        /// <summary>
        /// Creates a new role on the server in the current guild
        /// </summary>
        /// <param name="currentGuild">the current guild</param>
        /// <param name="currentRoleName">how the role should be named</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IRole> CreateRoleAsync(IGuild currentGuild, string currentRoleName)
        {
            try
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
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "RoleCreation:CreateRoleAsync");
                return null;
            }
        }
        
        /// <summary>
        /// Gets the roles from the guild matching to the users roleids
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
        private async Task<List<IRole>> GetUserRolesAsync(IGuildUser user, IGuild guild)
        {
            try
            {
                //get all role id's for the roles the user is in and match to the guild roles
                List<IRole> userRoles = new List<IRole>();
                foreach (IRole currentRole in guild.Roles)
                {
                    foreach (ulong roleId in user.RoleIds)
                    {
                        if (currentRole.Id == roleId)
                        {
                            userRoles.Add(currentRole);
                        }
                    }
                }
                return userRoles;
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "UserRoles:GetUserRolesAsync");
                return null;
            }
        }

        /// <summary>
        /// Removes not needed warning roles from the user in the guild
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="guild">the guild</param>
        /// <returns></returns>
        public async Task DeleteNotNeededWarningRolesFromUser(IGuildUser user, IGuild guild)
        {
            try
            {
                //get all roles the user currently has in the guild
                List<IRole> allRoles = await GetUserRolesAsync(user, guild);
            
                //get all warning roles
                List<IRole> warningRoles = new List<IRole>();
                foreach (IRole role in allRoles)
                {
                    if (role.Name.StartsWith(StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix))
                    {
                        warningRoles.Add(role);
                    }
                }
            
                //remove the warning roles from the user
                await user.RemoveRolesAsync(warningRoles);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "UserRoles:DeleteNotNeededWarningRolesFromUser");
            }
        }
        
        /// <summary>
        /// Returns all available warning roles on a specific guild
        /// </summary>
        /// <param name="guild">the current guild</param>
        /// <returns></returns>
        public Task<List<IRole>> GetAllAvailableWarningRolesInGuild(IGuild guild)
        {
            List<IRole> availableWarningRoles = new List<IRole>();

            foreach (IRole role in guild.Roles)
            {
                if (role.Name.StartsWith(StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix))
                {
                    availableWarningRoles.Add(role);
                }
            }
            return Task.FromResult(availableWarningRoles);
        }
    }
}
