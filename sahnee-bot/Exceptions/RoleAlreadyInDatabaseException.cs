using System;
using sahnee_bot.Logging;

namespace sahnee_bot.Exceptions
{
    public class RoleAlreadyInDatabaseException : Exception
    {
        //Variables
        private readonly Logger _logger = new Logger();
        
        public RoleAlreadyInDatabaseException(string message) : base(message)
        {
            _logger.Log(message, LogLevel.Error);
        }
    }
}
