using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands
{
    public class UserStats : ModuleBase<SocketCommandContext>
    {
        //Variables
        private readonly UserStatsAction _userStatsAction = new UserStatsAction();

        [Command("userstats")]
        [RoleHandling(RoleTypes.WarningBotAdmin)]
        [Summary("Migrates the current Database scheme to the latest scheme")]
        public async Task UserStatsAsync([Summary("The user it's all about")]IGuildUser user)
        {
            await _userStatsAction.UserStatsAsync(user, Context.Guild, Context.Channel);
        }
    }
}
