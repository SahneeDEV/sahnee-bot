using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands
{
    public class AddAdminRole : ModuleBase<SocketCommandContext>
    {
        //variables
        private readonly Logger _logger = new Logger();
        private readonly AddRoleAction _addRoleAction = new AddRoleAction();

        /// <summary>
        /// Adds a role with the admin privileges
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [Command("addadminrole")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        public async Task AddRoleAsync(
            [Summary("The role that will be added")]IRole role)
        {
            try
            {
                await _addRoleAction.AddRoleActionAsync(
                    Context.Channel, role, RoleTypes.WarningBotAdmin, Context.Guild, Context.User);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "AddModRole:AddRoleAsync:Role");
            }
        }
    }
}
