using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Database;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands
{
    public class GetRandomWarn : ModuleBase<SocketCommandContext>
    {
        //variables
        private readonly Logger _logger = new Logger();
        private readonly GetRandomWarnUnwarnAction _getRandomWarnUnwarnAction = new GetRandomWarnUnwarnAction();

        /// <summary>
        /// Gets a random warning from a specific user
        /// </summary>
        /// <param name="user">the user it's all about</param>
        [Command("randomwarn")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        private async Task GetRandomWarnUserAsync([Summary("The user to get one random warn from")] IUser user)
        {
            try
            {
                await _getRandomWarnUnwarnAction.GetRandomWarnUnwarnActionUserAsync(
                Context.Channel, user, Context.Guild, WarningType.Warning);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "GetRandomWarn:GetRandomWarnUserAsync");
            }
        }

        /// <summary>
        /// Gets a random warning from a guild from a random user
        /// </summary>
        /// <param name="role"></param>
        [Command("randomwarn")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        private async Task GetRandomWarnRoleAsync([Summary("The role to the a random user and the a random warn from")] IRole role)
        {
            try
            {
                await _getRandomWarnUnwarnAction.GetRandomWarnUnwarnActionGroupAsync(
                    Context.Channel, role, Context.Guild, WarningType.Warning);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "GetRandomWarn:GetRandomWarnRoleAsync");
            }
        }
    }
}
