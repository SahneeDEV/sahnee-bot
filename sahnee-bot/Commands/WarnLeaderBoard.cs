using System.Threading.Tasks;
using Discord.Commands;
using sahnee_bot.commands.CommandActions;
using sahnee_bot.RoleSystem;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class WarnLeaderBoard : ModuleBase<SocketCommandContext>
    {
        //Variables
        private readonly Logging _logging = new Logging();
        private readonly WarnLeaderBoardAction _warnLeaderBoardAction = new WarnLeaderBoardAction();

        #region Commands

         /// <summary>
         /// Command warnloaderboard
         /// shows the leaderboard from the database
         /// </summary>
         /// <returns></returns>
         [Command("warnleaderboard")]
         [RoleHandling(RoleTypes.WarningBotAdmin)]
         [Summary("Returns a fancy leaderboard of the users with the highest warnings")]
         public async Task LeaderBoardAsync()
         {
             try
             {
                 await StaticLock.AquireWarningAsync();
                 await this._warnLeaderBoardAction.ExecuteWarnLeaderBoardAsync(null, Context);
             }
             finally
             {
                 StaticLock.UnlockCommandWarning();
             }
         }
 
         /// <summary>
         /// Command warnloaderboard
         /// shows the leaderboard from the database
         /// </summary>
         /// <param name="amount">a custom amount</param>
         /// <returns></returns>
         [Command("warnleaderboard")]
         [RoleHandling(RoleTypes.WarningBotAdmin)]
         [Summary("Returns a fancy leaderboard of the users with the highest warnings with a custom amount")]
         public async Task LeaderBoardAsync([Summary("The custom amount of users to print in the history")]uint amount)
         {
             try
             {
                 await StaticLock.AquireWarningAsync();
                 await this._warnLeaderBoardAction.ExecuteWarnLeaderBoardAsync(amount, Context);
             }
             finally
             {
                 StaticLock.UnlockCommandWarning();
             }
         }

        #endregion
    }
}
