using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database.Standards;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands.CommandActions
{
    public class AddRoleAction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        private readonly AddRoleToDatabase _addRoleToDatabase = new AddRoleToDatabase();
        
        /// <summary>
        /// Adds a role to the database so it can be used by the guild
        /// </summary>
        /// <param name="channel">the channel to send feedback to</param>
        /// <param name="newRole">the new role</param>
        /// <param name="roleType">type of the role, like mod or admin</param>
        /// <param name="guild">id of the guild</param>
        /// <param name="user">the user that added the role</param>
        /// <returns></returns>
        public async Task AddRoleActionAsync(ISocketMessageChannel channel, IRole newRole, RoleTypes roleType, IGuild guild, IUser user)
        {
            try
            {
                //add the role to the database
                if (!await _addRoleToDatabase.AddRoleToDatabaseAsync(guild.Id, newRole, roleType))
                {
                    await channel.SendMessageAsync(
                        "🤔 A role with this ID already exists for you guild. I can't add it twice."
                        );
                    return;
                }
                
                //feedback to the user
                await channel.SendMessageAsync(
                    $"😉 Successfully added <@&{newRole.Id}> as usable Role. " +
                    $"\nYou may now assign this role if you haven't done so far." +
                    $"\nThis role can now executes commands with the " +
                    (roleType == RoleTypes.WarningBotMod ? "Moderator" : "Administrative") +
                    $" permissions");
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "AddRoleAction:AddRoleActionAsync");
                await channel.SendMessageAsync(
                    $"😕 Unfortunately I was not able to add your Role to be used as a valid role.");
            }
        }
    }
}
