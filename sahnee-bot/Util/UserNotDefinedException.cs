using System;
using Discord.Commands;
using Discord.WebSocket;

namespace sahnee_bot.Util
{
    public class UserNotDefinedException : Exception
    {
        //Variables
        private readonly Logging _logging = new Logging();
        
        public UserNotDefinedException()
        {
            
        }
        
        public UserNotDefinedException(string message, ISocketMessageChannel channel): base(message)
        {
            //Log to the console and write to the channel
            channel.SendMessageAsync("An Error Occured: " + message);
            _logging.LogToConsoleBase("An Error Occured: " + message);
        }
    }
}
