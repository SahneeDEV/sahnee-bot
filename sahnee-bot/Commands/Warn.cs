using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class Warn : ModuleBase<SocketCommandContext>
    {
        //Variables
        //private readonly RoleInformation _roleInformation = new RoleInformation();
        //private readonly RoleCreation _roleCreation = new RoleCreation();
        //private readonly RoleUserInteraction _roleUserInteraction = new RoleUserInteraction();
        //private readonly Logging _logging = new Logging();
        private WarnAction _warnAction = new WarnAction();

        #region Commands

        /// <summary>
        /// Command warn
        /// warns a user WITH a reason
        /// </summary>
        /// <param name="user">the user that will be warned</param>
        /// <param name="reason">the reason why the user will be warned</param>
        /// <returns></returns>
        [Command("warn")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Warns a specific user in his current guild")]
        public async Task WarnAsync([Summary("The user that will be warned")]IGuildUser user
            , [Summary("The reason why the user had been warned")][Remainder] string reason)
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
                    await this._warnAction.WarnAsync(user, reason, Context.Guild, Context.Channel, Context.Message);
                }
                finally
                {
                    StaticLock.UnlockCommandWarning();
                }
            }
        
        #endregion
        
    }
}
