using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.Configuration;
using sahnee_bot.Database.Standards;
using sahnee_bot.Logging;

namespace sahnee_bot.RoleSystem
{
    public class RoleHandling : PreconditionAttribute
    {
        //Variables
        private readonly Logger _logger = new Logger();
        private readonly List<string> _allRolesForCommand = new List<string>();
        private readonly GetAllRolesFromDatabase _getAllRolesFromDatabase = new GetAllRolesFromDatabase();
        private readonly RoleTypes _commandRoletype;

        /// <summary>
        /// Constructor for all our roles
        /// </summary>
        /// <param name="roleType">The needed roletype</param>
        public RoleHandling(RoleTypes roleType)
        {
            //Fill the required roles list
            _commandRoletype = roleType;
        }

        /// <summary>
        /// In Time loads the guilds roles
        /// </summary>
        /// <param name="guildId">the guilds id</param>
        /// <param name="roleType">the minimum required roletype</param>
        /// <returns></returns>
        private async void HotLoadGuildSpecificRoles(ulong guildId, RoleTypes roleType)
        {
            try
            {
                //admin
                if (roleType == RoleTypes.WarningBotAdmin)
                {
                    //adds the database roles to the list
                    _allRolesForCommand.AddRange(await _getAllRolesFromDatabase.GetAllRolesFromDatabaseAsync(guildId, roleType));
                }
                if (roleType == RoleTypes.WarningBotMod)
                {
                    //adds the database roles to the list
                    _allRolesForCommand.AddRange(await _getAllRolesFromDatabase.GetAllRolesFromDatabaseAsync(guildId, roleType));
                    
                    //add the admins aswell
                    _allRolesForCommand.AddRange(await _getAllRolesFromDatabase.GetAllRolesFromDatabaseAsync(guildId, RoleTypes.WarningBotAdmin));
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message,LogLevel.Error, "RoleHandling:HotLoadGuildSpecificRoles");
            }
        }

        /// <summary>
        /// Override of the default permission check
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            try
            {
                //allow admins to execute every command
                //check if the user is a admin
                if (context.User is SocketGuildUser guildAdmin)
                {
                    if (guildAdmin.GuildPermissions.Administrator)
                    {
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    }
                }
                
                //hot-load the guild specific roles
                HotLoadGuildSpecificRoles(context.Guild.Id, _commandRoletype);

                //Check if the user is a Guild User
                if (context.User is SocketGuildUser guildUser)
                {
                    //Check if the user is in one of the necessary roles
                    foreach (var roleName in _allRolesForCommand)
                    {
                        if (guildUser.Roles.Any(r => r.Name.ToLower() == roleName.ToLower()))
                        {
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.Message,LogLevel.Error, "RoleHandling:CheckPermissionsAsync");
            }

            context.Channel.SendMessageAsync("You don't have enough permissions.");
            _logger.Log($"User {context.User.Id} tried to execute a command with insufficient permissions"
                , LogLevel.Debug);
            return Task.FromResult(PreconditionResult.FromError($"..."));
        }
    }
}
