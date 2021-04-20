using System;
using System.Threading.Tasks;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class ChangePrefix : ModuleBase<SocketCommandContext>
    {
        
        //Variables
        private readonly Logger _logger = new Logger();
        ChangePrefixAction _changePrefixAction = new ChangePrefixAction();
        
        /// <summary>
        /// Command changeprefix
        /// changes the prefix for a guild
        /// </summary>
        /// <param name="newPrefix">the new prefix just a Char</param>
        /// <returns></returns>
        [Command("changeprefix")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Changes the prefix for all commands for the given guild")]
        public async Task ChangePrefixAsync([Summary("the new prefix")][Remainder] string newPrefix)
        {
            try
            {
                await _changePrefixAction.ChangePrefixActionAsync(Context.Guild, Context.Channel, newPrefix);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "ChangePrefix:ChangePrefixAsync");
            }
        }
    }
}
