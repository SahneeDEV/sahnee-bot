using System;
using System.Threading.Tasks;

namespace sahnee_bot.UserPunishment
{
    /// <summary>
    /// Represents a generic PunishmentReason
    /// </summary>
    public interface IPunishmentReason
    {
        //Variables
        
        /// <summary>
        /// Gets the name of the PunishmentReason
        /// </summary>
        string PunishmentReasonName { get; }
        /// <summary>
        /// Gets the message a user will receive of the PunishmentReason
        /// </summary>
        string PunishmentMessage { get; }
        
        
        //Methods
        
        /// <summary>
        /// Creates a new Task for the Punishment routine
        /// </summary>
        /// <returns></returns>
        Task PunishmentExecution();
    }
}
