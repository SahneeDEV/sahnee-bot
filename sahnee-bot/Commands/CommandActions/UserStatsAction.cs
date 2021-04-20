using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;
using sahnee_bot.UserInformation;

namespace sahnee_bot.commands.CommandActions
{
    public class UserStatsAction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        private readonly RoleInformation _roleInformation = new RoleInformation();
        private readonly UserRoles _userRoles = new UserRoles();
        
        /// <summary>
        /// Gets the stats for the user requested
        /// </summary>
        /// <param name="user">user to get information about</param>
        /// <param name="guild">current guild</param>
        /// <param name="channel">current channel</param>
        /// <returns></returns>
        public async Task UserStatsAsync(IGuildUser user, IGuild guild, ISocketMessageChannel channel)
        {
            try
            {
                //user information
                WarningBotCurrentStatesSchema warningBotCurrentStatesSchema = StaticDatabase.WarningCurrentStateCollection()
                    .Query()
                    .Where(u => u.UserId == user.Id).First();
                
                //Send all information for the current user
                await channel.SendMessageAsync($"Selected user: {user.Nickname}" +
                                               $"\nUser current warnings in database: {warningBotCurrentStatesSchema.Number}" +
                                               "\nNote: This could have been changed already. Warnings/Unwarns could have been added in the mean time.");
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync($"😭 I was unable to get some information! {e.Message}");
                await _logger.Log(e.Message, LogLevel.Error, "UserStatsAction:UserStatsAsync");
            }
        }
    }
}
