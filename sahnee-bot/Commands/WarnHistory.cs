using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands
{
    public class WarnHistory : ModuleBase<SocketCommandContext>
    {
        
        //Variables
        private readonly WarnHistoryAction _warnHistoryAction = new WarnHistoryAction();
        private readonly Logger _logger = new Logger();

        #region Commands

        /// <summary>
        /// Command warnhistory
        /// shows the warning history of a specific user
        /// </summary>
        /// <returns></returns>
        [Command("warnhistory")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Gets the warning history of a specific user")]
        [Alias("warnhistroy")]
        public async Task WarnAsync([Summary("The user all the information will be searched for")]IGuildUser user)
        {
            try
            {
                await this._warnHistoryAction.WarnHistoryAsync(user, Context.Guild, Context.Channel, Context.Message);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
            }
        }
        
        /// <summary>
        /// Command warnhistory
        /// shows the warning history of a specific user
        /// </summary>
        /// <returns></returns>
        [Command("warnhistory")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Gets the warning history of a specific user and the amount of entries from the database")]
        [Alias("warnhistroy")]
        public async Task WarnAsync([Summary("The user all the information will be searched for")]IGuildUser user, [Summary("The amount of entries that should be showed")]int amount)
        {
            try
            {
                await this._warnHistoryAction.WarnHistoryAsync(user, Context.Guild, Context.Channel, Context.Message, (uint)amount);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// Command warnhistory
        /// shows the warning history of a specific user
        /// </summary>
        /// <param name="user">the user whos history will be shown</param>
        /// <param name="param">a valid parameter</param>
        /// <returns></returns>
        [Command("warnhistory")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Gets all warning entries from a specific user")]
        [Alias("warnhistroy")]
        public async Task WarnAsync([Summary("The user all the information will be searched for")]IGuildUser user, [Summary("The string all, to get all available warnings for the user")]string param)
        {
            try
            {
                await this._warnHistoryAction.WarnHistoryAsync(user, Context.Guild, Context.Channel, Context.Message, param);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
            }
        }
        
        /// <summary>
        /// Command warnhistory
        /// shows the warning history with a fixed amount
        /// </summary>
        /// <param name="amount">the amount of items to show</param>
        /// <returns></returns>
        [Command("warnhistory")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Gets all warning entries from a specific user")]
        [Alias("warnhistroy")]
        public async Task WarnAsync([Summary("The user all the information will be searched for")]int amount)
        {
            //make negative numbers positive
            if (amount <0)
            {
                // ReSharper disable once IntVariableOverflowInUncheckedContext
                amount = amount * -1;
            }
            try
            {
                await this._warnHistoryAction.WarnHistoryAsync(null, Context.Guild, Context.Channel, Context.Message, (uint)amount);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "WarnHistory:WarnAsync");
            }
        }
        #endregion
    }
}
