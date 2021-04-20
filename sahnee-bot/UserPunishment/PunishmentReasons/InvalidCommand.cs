namespace sahnee_bot.UserPunishment.PunishmentReasons
{
    public class InvalidCommand
    {
        //Variables
        
        /// <summary>
        /// Gets the name of the invalidcommand
        /// </summary>
        public string InvalidCommandReasonName { get; }
        /// <summary>
        /// Gets the message a user will receive as punishment
        /// </summary>
        public string InvalidCommandMessage { get; }

        public InvalidCommand()
        {
            InvalidCommandReasonName = "Unknown command";
            InvalidCommandMessage = "the command you entered is not valid. Please check the available parameters";
        }
    }
}
