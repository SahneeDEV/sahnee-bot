using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands
{
    public class AddModRole : ModuleBase<SocketCommandContext>
    {
        //variables
        private readonly Logger _logger = new Logger();
        private readonly AddRoleAction _addRoleAction = new AddRoleAction();

        /// <summary>
        /// Adds a role with the mod privileges
        /// </summary>
        /// <param name="role">the role which should be added</param>
        /// <returns></returns>
        [Command("addmodrole")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        public async Task AddRoleAsync(
            [Summary("The role that will be used")]IRole role)
        {
            try
            {
                await _addRoleAction.AddRoleActionAsync(
                    Context.Channel, role, RoleTypes.WarningBotMod, Context.Guild, Context.User);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "AddModRole:AddRoleAsync:Role");
            }
        }
    }
}
