using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Configuration;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;

namespace sahnee_bot.RoleSystem
{
    public class RoleInformation
    {
        ////Variables
        //private readonly Logger _logger = new Logger();
        //private readonly RoleCreation _roleCreation = new RoleCreation();
        //
        ///// <summary>
        ///// Gets the highest warning role number of a user
        ///// </summary>
        ///// <param name="user">the user that gets warned</param>
        ///// <param name="guild">current guild</param>
        ///// <returns>highest role as an integer</returns>
        //public async Task<uint> HighestWarningRoleNumberOfUserAsync(IGuildUser user, IGuild guild)
        //{
        //    //get the current warning number
        //    try
        //    {
        //        List<WarningBotCurrentStatesSchema> allEntries = StaticDatabase.WarningCurrentStateCollection().Query()
        //            .Where(u => u.UserId == user.Id && u.GuildId == guild.Id)
        //            .ToList();
        //        //check if the list has more than 1 entry, if so, something is fishy
        //        if (allEntries.Count == 1)
        //        {
        //            return allEntries[0].Number;
        //        }
        //        if (allEntries.Count == 0)
        //        {
        //            //new user
        //            return 0;
        //        }
        //        //fallback method, because no or multiple entries should never ever exist
        //        throw new InvalidDataException();
        //    }
        //    catch (Exception e)
        //    {
        //        await _logger.Log("An Error Occured while trying to the the warning role from the database", LogLevel.Error, "RoleInformation:HighestWarningRoleNumberOfUserAsync");
        //        return await FallbackUserCurrentlyHighestNumberOfUser(user, guild);
        //    }
        //}
//
        ///// <summary>
        ///// Fallback method to try to get the current warning role from the users roles
        ///// </summary>
        ///// <param name="user"></param>
        ///// <param name="guild"></param>
        ///// <returns></returns>
        //public async Task<uint> FallbackUserCurrentlyHighestNumberOfUser(IGuildUser user, IGuild guild)
        //{
        //    //get all warning roles
        //    //List<SocketRole> warningRoles = user.Roles.Where(role => role.Name.StartsWith(StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix)).ToList();
        //    List<IRole> warningRoles = await this.GetUserRolesAsync(user, guild);
        //    //if user does not have any warning role
        //    if (warningRoles.Count == 0)
        //    {
        //        return 0;
        //    }
//
        //    //Convert all role names to a sting
        //    List<string> warningRolesString = new List<string>();
        //    foreach (var role in warningRoles)
        //    {
        //        warningRolesString.Add(role.Name);
        //    }
//
        //    //remove the 'WarningPrefix'
        //    for (int i = 0; i < warningRolesString.Count; i++)
        //    {
        //        warningRolesString[i] = warningRolesString[i]
        //            .Replace(StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix, "")
        //            .Replace(" ", "");
        //    }
//
        //    //try to parse the remaining content into integer
        //    List<int> warningRolesInteger = new List<int>();
        //    for (int i = 0; i < warningRolesString.Count; i++)
        //    {
        //        int temp = -10;
        //        int.TryParse(warningRolesString[i], out temp);
        //        if (temp != -10)
        //        {
        //            warningRolesInteger.Add(temp);
        //        }
        //    }
//
        //    //get the highest number
        //    uint hightestWarningRole = (uint) warningRolesInteger.Max();
        //    return hightestWarningRole;
        //}
//
        ///// <summary>
        ///// Gets the roles from the guild matching to the users roleids
        ///// </summary>
        ///// <param name="user"></param>
        ///// <param name="guild"></param>
        ///// <returns></returns>
        //private async Task<List<IRole>> GetUserRolesAsync(IGuildUser user, IGuild guild)
        //{
        //    //get all role id's for the roles the user is in and match to the guild roles
        //    List<IRole> userRoles = new List<IRole>();
        //    foreach (IRole currentRole in guild.Roles)
        //    {
        //        foreach (ulong roleId in user.RoleIds)
        //        {
        //            if (currentRole.Id == roleId)
        //            {
        //                userRoles.Add(currentRole);
        //            }
        //        }
        //    }
        //    return userRoles;
        //}
        //
        //
        ///// <summary>
        ///// Gets the users currently highest warning role as a IRole
        ///// </summary>
        ///// <param name="user">the user that gets warned</param>
        ///// <param name="guild"></param>
        ///// <returns>the IRole of the currently highest waring of the user</returns>
        //public async Task<IRole> HighestWarningRoleRoleUserAsync(IGuildUser user, IGuild guild)
        //{
        //    try
        //    {
        //        //get the current warning role number
        //        List<WarningBotCurrentStatesSchema> allEntries = StaticDatabase.WarningCurrentStateCollection().Query()
        //            .Where(u => u.UserId == user.Id && u.GuildId == guild.Id)
        //            .ToList();
        //        
        //        //get all roles the user currently is in
        //        List<IRole> allRoles = await this.GetUserRolesAsync(user, guild);
        //        //get the role that matches
        //        string roleToFind = StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix +
        //                            allEntries[0].Number;
        //        IRole oldRole = allRoles.Find(role => role.Name == roleToFind);
        //        //if user does not have the correct role
        //        if (oldRole == null)
        //        {
        //            //create the role temporarily
        //            oldRole = await _roleCreation.CreateRoleAsync(guild, roleToFind);
        //        }
        //        return oldRole;
        //    }
        //    catch (Exception e)
        //    {
        //        await _logger.Log("An Error Occured while trying to the the warning role from the database", LogLevel.Error, "RoleInformation:HighestWarningRoleRoleUserAsync");
        //        return await this.FallbackUserCurrentlyHighestWarningRole(user, guild);
        //    }
        //}
//
        ///// <summary>
        ///// Fallback method to try to get the role by guild role
        ///// </summary>
        ///// <param name="user"></param>
        ///// <param name="guild"></param>
        ///// <returns></returns>
        //private async Task<IRole> FallbackUserCurrentlyHighestWarningRole(IGuildUser user, IGuild guild)
        //{
        //    //get all roles the user currently is in
        //    List<IRole> allRoles = await this.GetUserRolesAsync(user, guild);
        //    List<string> allRoleNames = new List<string>();
        //    foreach (var role in allRoles)
        //    {
        //        allRoleNames.Add(role.Name);
        //    }
        //    //cut the warningprefix
        //    for (int i = 0; i < allRoleNames.Count; i++)
        //    {
        //        allRoleNames[i] = allRoleNames[i].Replace(StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix, "");
        //    }
        //    
        //    int currentHighestNumber = 0;
        //    int currentHighestIndex = 0;
        //    for (int i = 0; i < allRoleNames.Count; i++)
        //    {
        //        int roleNumber = 0;
        //        int.TryParse(allRoleNames[i], out roleNumber);
        //        if (roleNumber >= currentHighestNumber)
        //        {
        //            currentHighestNumber = roleNumber;
        //            currentHighestIndex = i;
        //        }
        //    }
        //    
        //    //Return the role at the index that got found as highest role
        //    return allRoles[currentHighestIndex];
        //}
//
        ///// <summary>
        ///// Returns all available warning roles on a specific guild
        ///// </summary>
        ///// <param name="guild">the current guild</param>
        ///// <returns></returns>
        //public async Task<List<IRole>> GetAllAvailableWarningRolesInGuild(IGuild guild)
        //{
        //    List<IRole> availableWarningRoles = new List<IRole>();
//
        //    foreach (IRole role in guild.Roles)
        //    {
        //        if (role.Name.StartsWith(StaticConfiguration.GetConfiguration().WarningBot.WarningPrefix))
        //        {
        //            availableWarningRoles.Add(role);
        //        }
        //    }
        //    return availableWarningRoles;
        //}
        
    }
}
