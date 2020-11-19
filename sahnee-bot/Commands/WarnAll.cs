using System.Threading.Tasks;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class WarnAll : ModuleBase<SocketCommandContext>
    {
        //Variables
        private WarnAllAction _warnAllAction = new WarnAllAction();
        #region Commands

        /// <summary>
        /// Command warnall
        /// warns all users WITH a reason
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        [Command("warnall")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Warns all users on the server")]
        public async Task WarnAllAsync([Summary("the reason why all users will be warned")][Remainder] string reason)
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
                await _warnAllAction.WarnAllAsync(reason, Context.Guild, Context.Channel, Context.Message);
            }
            finally
            {
                StaticLock.UnlockCommandWarning();
            }
        }
        
        #endregion
        
    }
}
