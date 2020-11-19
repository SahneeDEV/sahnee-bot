using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.Util;

namespace sahnee_bot.RoleSystem
{
    public class RoleHandling : PreconditionAttribute
    {
        //Variables
        private string _roleName;
        private string[] _roleNames;
        private RoleTypes _currentRoleType;
        /// <summary>
        /// Constructor for all our roles
        /// </summary>
        /// <param name="roleType">The needed roletype</param>
        public RoleHandling(RoleTypes roleType)
        {
            //Fill in the variables based on the selected type
            if (roleType == RoleTypes.WarningBotAdmin)
            {
                if (StaticConfiguration.GetConfiguration().WarningBot.Admins.Length > 1)
                {
                    _roleNames = StaticConfiguration.GetConfiguration().WarningBot.Admins;
                }
                _roleName = StaticConfiguration.GetConfiguration().WarningBot.Admins[0];
            }
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            //Check if the user is a Guild User
            if (context.User is SocketGuildUser guildUser)
            {
                //Check what kind of rolehandling has been instantiated
                if (_roleName != null)
                {
                    //Check if the user is in the necessary role
                    if (guildUser.Roles.Any(r => r.Name.ToLower() == _roleName.ToLower()))
                    {
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    }
                }
                if (_roleNames != null)
                {
                    //Check if the user is in one of the necessary roles
                    foreach (var roleName in _roleNames)
                    {
                        if (guildUser.Roles.Any(r => r.Name.ToLower() == roleName.ToLower()))
                        {
                            return Task.FromResult(PreconditionResult.FromSuccess());
                        }
                    }
                }
            }
            context.Message.DeleteAsync();
            return Task.FromResult(PreconditionResult.FromError($"You are missing the following {(_roleName != null ? "role: " + _roleName : "roles: " + _roleNames )}"));
        }
    }
}
