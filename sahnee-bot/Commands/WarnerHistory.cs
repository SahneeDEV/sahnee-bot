using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands
{
    public class WarnerHistory : ModuleBase<SocketCommandContext>
    {
        //Variables
        private readonly Logger _logger = new Logger();
        private readonly WarnerHistoryAction _warnerHistoryAction = new WarnerHistoryAction();

        /// <summary>
        /// Command warnerhistory
        /// </summary>
        /// <param name="user">the user to get everything from</param>
        [Command("warnerhistory")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Gets all warnings/unwarns a user issued")]
        public async Task WarnerHistoryAsync([Summary("The user all the information will be searched for")]IGuildUser user)
        {
            try
            {
                await _warnerHistoryAction.WarnerHistoryActionAsync(user, Context.Guild, Context.Channel);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "WarnerHistory:WarnerHistoryAsync");
            }
        }
        
        /// <summary>
        /// Command warnerhistory
        /// </summary>
        /// <param name="user">the user to get everything from</param>
        /// <param name="amount">the amount of warnings/unwarns to display</param>
        [Command("warnerhistory")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Gets all warnings/unwarns a user issued")]
        public async Task WarnerHistoryAsync([Summary("The user all the information will be searched for")]IGuildUser user, int amount)
        {
            try
            {
                await _warnerHistoryAction.WarnerHistoryActionAsync(user, Context.Guild, Context.Channel, (uint)amount);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "WarnerHistory:WarnerHistoryAsync");
            }
        }
    }
}
