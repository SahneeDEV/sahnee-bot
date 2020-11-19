using System.Threading.Tasks;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.Jobs.JobTasks;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class CleanupRoles : ModuleBase<SocketCommandContext>
    {
        //Variables
        private readonly CleanupWarningRolesAction _cleanupWarningRolesAction = new CleanupWarningRolesAction();
        
        #region Commands

        /// <summary>
        /// Command cleanuproles
        /// manually triggers the role warning cleanup script
        /// </summary>
        /// <returns></returns>
        [Command("cleanuproles")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Manually triggers the role warning cleanup script")]
        public async Task CleanupRolesAsync()
        {
            try
            {
                await StaticLock.AquireWarningAsync();
                await this._cleanupWarningRolesAction.CleanupWarningRoles(Context);
            }
            finally
            {
                StaticLock.UnlockCommandWarning();
            }
        }
        
        #endregion
    }
}
