using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class WarnHistory : ModuleBase<SocketCommandContext>
    {
        
        //Variables
        private readonly WarnHistoryAction _warnHistoryAction = new WarnHistoryAction();

        #region Commands

        /// <summary>
        /// Command warnhistory
        /// shows the warning history of a specific user
        /// </summary>
        /// <returns></returns>
        [Command("warnhistory")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Gets the warning history of a specific user")]
        public async Task WarnAsync([Summary("The user all the information will be searched for")]IGuildUser user)
        {
            try
            {
                await StaticLock.AquireWarningHistroyAsync();
                await this._warnHistoryAction.WarnHistoryAsync(user, Context.Guild, Context.Channel, Context.Message);
            }
            finally
            {
                StaticLock.UnlockCommandWarnHistory();
            }
        }
        
        /// <summary>
        /// Command warnhistory
        /// shows the warning history of a specific user
        /// </summary>
        /// <returns></returns>
        [Command("warnhistory")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Gets the warning history of a specific user and the amount of entries from the database")]
        public async Task WarnAsync([Summary("The user all the information will be searched for")]IGuildUser user, [Summary("The amount of entries that should be showed")]int amount)
        {
            try
            {
                await StaticLock.AquireWarningHistroyAsync();
                await this._warnHistoryAction.WarnHistoryAsync(user, Context.Guild, Context.Channel, Context.Message, (uint)amount);
            }
            finally
            {
                StaticLock.UnlockCommandWarnHistory();
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
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Gets all warning entries from a specific user")]
        public async Task WarnAsync([Summary("The user all the information will be searched for")]IGuildUser user, [Summary("The string all, to get all available warnings for the user")]string param)
        {
            try
            {
                await StaticLock.AquireWarningHistroyAsync();
                await this._warnHistoryAction.WarnHistoryAsync(user, Context.Guild, Context.Channel, Context.Message, param);
            }
            finally
            {
                StaticLock.UnlockCommandWarnHistory();
            }
        }

        #endregion

        #region Duplicates for silly me not being able to write history and instead write histroy -> this prevents me getting warned 😋

                /// <summary>
        /// Command warnhistory
        /// shows the warning history of a specific user
        /// </summary>
        /// <returns></returns>
        [Command("warnhistroy")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Gets the warning history of a specific user")]
        public async Task WarnDuplAsync([Summary("The user all the information will be searched for")]IGuildUser user)
        {
            try
            {
                await StaticLock.AquireWarningHistroyAsync();
                await this._warnHistoryAction.WarnHistoryAsync(user, Context.Guild, Context.Channel, Context.Message);
                await Context.Channel.SendMessageAsync("Well... This command turned out to be correct🤔");
            }
            finally
            {
                StaticLock.UnlockCommandWarnHistory();
            }
        }
        
        /// <summary>
        /// Command warnhistory
        /// shows the warning history of a specific user
        /// </summary>
        /// <returns></returns>
        [Command("warnhistroy")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Gets the warning history of a specific user and the amount of entries from the database")]
        public async Task WarnDuplAsync([Summary("The user all the information will be searched for")]IGuildUser user, [Summary("The amount of entries that should be showed")]int amount)
        {
            try
            {
                await StaticLock.AquireWarningHistroyAsync();
                await this._warnHistoryAction.WarnHistoryAsync(user, Context.Guild, Context.Channel, Context.Message, (uint)amount);
                await Context.Channel.SendMessageAsync("Well... This command turned out to be correct🤔");
            }
            finally
            {
                StaticLock.UnlockCommandWarnHistory();
            }
        }

        /// <summary>
        /// Command warnhistory
        /// shows the warning history of a specific user
        /// </summary>
        /// <param name="user">the user whos history will be shown</param>
        /// <param name="param">a valid parameter</param>
        /// <returns></returns>
        [Command("warnhistroy")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Gets all warning entries from a specific user")]
        public async Task WarnDuplAsync([Summary("The user all the information will be searched for")]IGuildUser user, [Summary("The string all, to get all available warnings for the user")]string param)
        {
            try
            {
                await StaticLock.AquireWarningHistroyAsync();
                await this._warnHistoryAction.WarnHistoryAsync(user, Context.Guild, Context.Channel, Context.Message, param);
                await Context.Channel.SendMessageAsync("Well... This command turned out to be correct🤔");
            }
            finally
            {
                StaticLock.UnlockCommandWarnHistory();
            }
        }

        #endregion
    }
}
