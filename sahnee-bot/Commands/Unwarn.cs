using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class Unwarn : ModuleBase<SocketCommandContext>
    {
        //Variables
        private UnwarnAction _unwarnAction = new UnwarnAction();
        
        #region Commands

        /// <summary>
        /// Command unwarn
        /// unwarns a user WITH a reason
        /// </summary>
        /// <param name="user">the user that will be unwarned</param>
        /// <param name="reason">the reason why the user will be unwarned</param>
        /// <returns></returns>
        [Command("unwarn")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Unwarns a specific user in his current guild WITH a reason")]
        public async Task UnwarnAsync([Summary("The user that will be unwarned")]IGuildUser user
            , [Summary("The reason why the user will be unwarned")][Remainder] string reason)
        {
            //prevent too large messages
            if (reason.Length > StaticInternalConfiguration.CharacterLimitMessage)
            {
                await Context.Channel.SendMessageAsync($"🤯 Wow that reason is way too long. Your character count is {reason.Length} but the maximum character count is {StaticInternalConfiguration.CharacterLimitMessage}.");
                return;
            }
            try
            {
                await StaticLock.AquireWarningAsync();
                await _unwarnAction.UnwarnAsync(user, Context.Guild, Context.Channel, Context.Message, reason);
            }
            finally
            {
                StaticLock.UnlockCommandWarning();
            }
        }
        
        /// <summary>
        /// Command unwarn
        /// unwarns a user WITHOUT a reason
        /// </summary>
        /// <param name="user">the user that will be unwarned</param>
        /// <returns></returns>
        [Command("unwarn")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Unwarns a specific user in his current guild WITHOUT a reason")]
        public async Task UnwarnAsync([Summary("The user that will be unwarned")]SocketGuildUser user)
        {
            try
            {
                await StaticLock.AquireWarningAsync();
                await _unwarnAction.UnwarnAsync(user, Context.Guild, Context.Channel, Context.Message);
            }
            finally
            {
                StaticLock.UnlockCommandWarning();
            }
        }
        
        #endregion
        
    }
}
