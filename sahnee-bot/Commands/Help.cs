using System.Threading.Tasks;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        //Variables
        private readonly HelpAction _helpAction = new HelpAction();

        [Command("help")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Shows a dialog with all available commands")]
        public async Task HelpAsync()
        {
            await _helpAction.HelpActionAsync(Context.Channel);
            
        }
    }
}
