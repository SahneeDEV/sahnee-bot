using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class WarningsToday : ModuleBase<SocketCommandContext>
    {
        //Variables
        private readonly WarningsTodayAction _warningsTodayAction = new WarningsTodayAction();
        private readonly Logger _logger = new Logger();

        #region Commands

        /// <summary>
        /// Command warningstoday
        /// shows all warnings a specific user got today
        /// </summary>
        /// <param name="user">the user who's warnings will be printed out</param>
        /// <returns></returns>
        [Command("warningstoday")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Shows all warnings for the current guild and a specific user that have been issued today")]
        public async Task WarningsTodayAsync([Summary("the user the warnings will be showed for")]IGuildUser user)
        {
            try
            {
                await _warningsTodayAction.WarningsTodayAsync(user, Context.Guild, Context.Channel);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// Command warningstoday
        /// shows all warnings for every user today
        /// </summary>
        /// <returns></returns>
        [Command("warningstoday")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Shows all warnings for the current guild that have been issued today")]
        public async Task WarningsTodayAsync()
        {
            try
            {
                await _warningsTodayAction.WarningsTodayAsync(Context.Guild, Context.Channel);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
            }
        }

        #endregion
    }
}
