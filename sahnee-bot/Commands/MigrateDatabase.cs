using System.Threading.Tasks;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class MigrateDatabase : ModuleBase<SocketCommandContext>
    {
        //Variables
        private readonly MigrateDatabaseAction _migrateDatabaseAction = new MigrateDatabaseAction();
        
        [Command("migratedatabase")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Migrates the current Database scheme to the latest scheme")]
        public async Task MigrateDatabaseAsync()
        {
            try
            {
                await StaticLock.AquireAllAsync();
                await _migrateDatabaseAction.MigrateDatabaseAsync(Context);
            }
            finally
            {
                StaticLock.UnlockAll();
            }
        }
    }
}
