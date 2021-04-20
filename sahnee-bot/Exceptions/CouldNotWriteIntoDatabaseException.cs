using System;
using sahnee_bot.Logging;

namespace sahnee_bot.Exceptions
{
    public class CouldNotWriteIntoDatabaseException : Exception
    {
        //Variables
        private readonly Logger _logger = new Logger();

        public CouldNotWriteIntoDatabaseException(string message) : base(message)
        {
            _logger.Log(message, LogLevel.Error);
        }
    }
}