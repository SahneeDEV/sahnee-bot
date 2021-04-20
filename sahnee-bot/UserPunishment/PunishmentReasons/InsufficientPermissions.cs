using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.commands.CommandActions;

namespace sahnee_bot.UserPunishment.PunishmentReasons
{
    public class InsufficientPermissions
    {
        //Variables
        
        /// <summary>
        /// Gets the name of the PunishmentReason
        /// </summary>
        public string PunishmentReasonName { get; }
        /// <summary>
        /// Gets the message a user will receive as punishment
        /// </summary>
        public string PunishmentMessage { get; }

        public InsufficientPermissions()
        {
            PunishmentReasonName = "Insufficient permissions";
            PunishmentMessage = "you are missing ";
        }
    }
}
