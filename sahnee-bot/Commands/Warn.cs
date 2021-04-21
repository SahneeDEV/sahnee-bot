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
    public class Warn : ModuleBase<SocketCommandContext>
    {
        //Variables
        private readonly WarnAction _warnAction = new WarnAction();
        private readonly WarnRoleAction _warnRoleAction = new WarnRoleAction();
        private readonly Logger _logger = new Logger();

        #region Commands

        /// <summary>
        /// Command warn
        /// warns a user WITH a reason
        /// </summary>
        /// <param name="user">the user that will be warned</param>
        /// <param name="reason">the reason why the user will be warned</param>
        /// <returns></returns>
        [Command("warn")]
        [RoleHandling(RoleTypes.WarningBotMod)]
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
                    await _warnAction.WarnAsync(user, reason, Context.Guild, Context.Channel, Context.Message);
                }
                catch (Exception e)
                {
                    await _logger.Log(e.Message, LogLevel.Error, "QueueManager:WarnAsync");
                }
            }

        /// <summary>
        /// Command warn
        /// warns all users within a group
        /// </summary>
        /// <param name="role">the role all users within that will be warned</param>
        /// <param name="reason">the reason why the users will be warned</param>
        [Command("warn")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Warns a specific user in his current guild")]
        public async Task WarnAsync([Summary("The group that will be warned")]IRole role
        ,[Summary("The reason why the group has been warned")][Remainder] string reason)
        {
            //prevent too large messages
            if (reason.Length > StaticInternalConfiguration.CharacterLimitMessage)
            {
                await Context.Channel.SendMessageAsync($"🤯 Wow that reason is way too long. Your character count is {reason.Length} but the maximum character count is {StaticInternalConfiguration.CharacterLimitMessage}.");
                return;
            }

            try
            {
                await _warnRoleAction.WarnRoleAsync(role, reason, Context.Guild, Context.Channel, Context.Message);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "QueueManager:WarnAsync");
            }
        }
        #endregion
    }
}
