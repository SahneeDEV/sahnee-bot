using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands
{
    public class RemoveAdminRole : ModuleBase<SocketCommandContext>
    {
        //variables
        private readonly Logger _logger = new Logger();
        private readonly RemoveRoleAction _removeRoleAction = new RemoveRoleAction();

        /// <summary>
        /// Removes a role with admin privileges
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [Command("removeadminrole")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        public async Task RemoveRoleAsync(
            [Summary("The role that will be removed")]IRole role)
        {
            try
            {
                await _removeRoleAction.RemoveRoleActionAsync(
                    Context.Channel, role, RoleTypes.WarningBotAdmin, Context.Guild);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "RemoveAdminRole:RemoveRoleAsync:Role");
            }
        }
        
        /// <summary>
        /// Just warns the user because he can't use a user as role
        /// </summary>
        /// <param name="user">the user the user gave and thought he would get away with</param>
        /// <returns></returns>
        [Command("removeadminrole")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        public async Task RemoveRoleAsync(
            [Summary("The role that will be used")]IUser user)
        {
            try
            {
                //TODO: punish the user because he can't assign a user as group
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "RemoveAdminRole:RemoveRoleAsync:User");
            }
        }

    }
}
