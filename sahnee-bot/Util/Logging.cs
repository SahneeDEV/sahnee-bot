using System;
using System.Threading.Tasks;
using Discord;

namespace sahnee_bot.Util
{
    public class Logging
    {
        /// <summary>
        /// Basic Logging to the console via the Discord.NET logging eventhandler
        /// </summary>
        /// <param name="message">the message</param>
        /// <returns></returns>
        public Task LogToConsole(LogMessage message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Basic logging to the console. Can be triggered manual
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task LogToConsoleBase(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " " + message);
            return Task.CompletedTask;
        }
    }
}
