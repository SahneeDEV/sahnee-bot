using System;
using System.Threading.Tasks;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands
{
    public class ListAdminRoles : ModuleBase<SocketCommandContext>
    {
        //variables
        private readonly Logger _logger = new Logger();
        private readonly ListRolesAction _listRolesAction = new ListRolesAction();
        
        /// <summary>
        /// Lists all available admin roles for the current guild
        /// </summary>
        /// <returns></returns>
        [Command("listadminroles")]
        public async Task ListAdminRolesAsync()
        {
            try
            {
                await _listRolesAction.ListRolesActionAsync(
                    Context.Channel, Context.Guild, RoleTypes.WarningBotAdmin);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "ListAdminRoles:ListAdminRolesAsync");
            }
        }
    }
}
