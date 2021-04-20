using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database.Standards;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;

namespace sahnee_bot.commands.CommandActions
{
    public class RemoveRoleAction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        private readonly RemoveRoleFromDatabase _removeRoleFromDatabase = new RemoveRoleFromDatabase();
        
        /// <summary>
        /// Removes a role from the database
        /// </summary>
        /// <param name="channel">the channel to send feedback to</param>
        /// <param name="oldRole">the role that will be removed</param>
        /// <param name="roleType">type of the role</param>
        /// <param name="guild">the guild it's all about</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task RemoveRoleActionAsync(ISocketMessageChannel channel, IRole oldRole, RoleTypes roleType, IGuild guild)
        {
            try
            {
                //remove the role from the database
                if (!await _removeRoleFromDatabase.RemoveRoleFromDatabaseAsync(guild.Id, oldRole, roleType))
                {
                    if (roleType == RoleTypes.WarningBotAdmin)
                    {
                        await channel.SendMessageAsync(
                            $"🤔 Either your role does not exist or it's the last remaining admin role available. One admin role has to exist at minimum. Nothing changed."
                        );
                    }
                    else
                    {
                        await channel.SendMessageAsync(
                            $"🤔 Your role does not exist in the database. Nothing changed."
                        );
                    }
                    return;
                }
                
                //feedback to the user
                await channel.SendMessageAsync(
                    $"😉 Successfully removed <@&{oldRole.Id}> as usable Role. "
                    );
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "RemoveRoleAction:RemoveRoleActionAsync");
                await channel.SendMessageAsync(
                    $"😕 Unfortunately I was not able to remove your Role from the database. Please contact the owners of the bot.");
            }
        }
    }
}
