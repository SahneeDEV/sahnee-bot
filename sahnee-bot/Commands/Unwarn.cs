using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class Unwarn : ModuleBase<SocketCommandContext>
    {
        //Variables
        private UnwarnAction _unwarnAction = new UnwarnAction();
        private readonly Logger _logger = new Logger();
        private UnwarnRoleAction _unwarnRoleAction = new UnwarnRoleAction();
        
        #region Commands

        /// <summary>
        /// Command unwarn
        /// unwarns a user WITH a reason
        /// </summary>
        /// <param name="user">the user that will be unwarned</param>
        /// <param name="reason">the reason why the user will be unwarned</param>
        /// <returns></returns>
        [Command("unwarn")]
        [RoleHandling(RoleTypes.WarningBotMod)]
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
                await _unwarnAction.UnwarnAsync(user, Context.Guild, Context.Channel, Context.Message, reason);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "Unwarn:UnwarnAsync");
            }
        }
        
        /// <summary>
        /// Command unwarn
        /// unwarns a user WITHOUT a reason
        /// </summary>
        /// <param name="user">the user that will be unwarned</param>
        /// <returns></returns>
        [Command("unwarn")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Unwarns a specific user in his current guild WITHOUT a reason")]
        public async Task UnwarnAsync([Summary("The user that will be unwarned")]SocketGuildUser user)
        {
            try
            {
                await _unwarnAction.UnwarnAsync(user, Context.Guild, Context.Channel, Context.Message);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "Unwarn:UnwarnAsync:NoReason");
            }
        }

        /// <summary>
        /// Command unwarn
        /// unwarns all users within a group
        /// </summary>
        /// <param name="role"></param>
        /// <param name="reason"></param>
        [Command("unwarn")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Unwarns all users within a role in his current guild WITH a reason")]
        public async Task UnwarnAsync([Summary("The group that will be unwarned")]IRole role
            , [Summary("The reason why the group has been unwarned")][Remainder] string reason)
        {
            //prevent too large messages
            if (reason.Length > StaticInternalConfiguration.CharacterLimitMessage)
            {
                await Context.Channel.SendMessageAsync($"🤯 Wow that reason is way too long. Your character count is {reason.Length} but the maximum character count is {StaticInternalConfiguration.CharacterLimitMessage}.");
                return;
            }
            try
            {
                await _unwarnRoleAction.UnwarnRoleAsync(role, 
                    Context.Guild, Context.Channel, Context.Message, reason);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "Unwarn:UnwarnAsync:Role");
            }
        }
        
        /// <summary>
        /// Command unwarn
        /// unwarns all users within a group WITHOUT a reason
        /// </summary>
        /// <param name="role"></param>
        /// <param name="reason"></param>
        [Command("unwarn")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Unwarns all users within a role in his current guild WITHOUT a reason")]
        public async Task UnwarnAsync([Summary("The group that will be unwarned")]IRole role)
        {
            try
            {
                await _unwarnRoleAction.UnwarnRoleAsync(role, 
                    Context.Guild, Context.Channel, Context.Message);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "Unwarn:UnwarnAsync:Role:NoReason");
            }
        }
        
        #endregion
        
    }
}
