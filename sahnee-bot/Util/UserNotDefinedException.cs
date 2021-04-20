using System;
using Discord.WebSocket;
using sahnee_bot.Logging;

namespace sahnee_bot.Util
{
    public class UserNotDefinedException : Exception
    {
        //Variables
        private readonly Logger _logger = new Logger();
        
        public UserNotDefinedException()
        {
            
        }
        
        public UserNotDefinedException(string message, ISocketMessageChannel channel): base(message)
        {
            //Log to the console and write to the channel
            channel.SendMessageAsync("An Error Occured: " + message);
            _logger.Log("An Error Occured: " + message, LogLevel.Error, "UserNotDefinedException:UserNotDefinedException");
        }
    }
}
