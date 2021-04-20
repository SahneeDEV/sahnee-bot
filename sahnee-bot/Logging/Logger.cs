#nullable enable
using System;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Configuration;

namespace sahnee_bot.Logging
{
    public class Logger
    {
        //Variables
        private LogLevel _logLevel = LogLevel.Warning;

        private void GetLogLevel()
        {
            try
            {
                _logLevel = (LogLevel)StaticConfiguration.GetConfiguration().General.LogLevel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Logger()
        {
            GetLogLevel();
        }

        //Multiple implementations of the Log() method
        /// <summary>
        /// Method to send the logs to
        /// </summary>
        /// <param name="message">the message to output to the console</param>
        /// <param name="logLevel">the level of the message. see LogLevel for further information</param>
        /// <returns></returns>
        public Task Log(string message, LogLevel logLevel)
        {
            SendLogToConsole(message, logLevel);
            return Task.CompletedTask;
        }

        public Task Log(string message, LogLevel logLevel, string className)
        {
            SendLogToConsole(message + " " + className, logLevel);
            return Task.CompletedTask;
        }

        /// <summary>
        /// For discord.NET logs
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Log(LogMessage message)
        {
            SendLogToConsole(message.ToString(), LogLevel.Critical);
                return Task.CompletedTask;
        }


        /// <summary>
        /// Prefix generation
        /// </summary>
        /// <returns>the prefix</returns>
        private string LogPrefix()
        {
            return DateTime.Now.ToString("HH:mm:ss") + " ";
        }

        /// <summary>
        /// Method for sending the log message to the console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        private Task SendLogToConsole(string message, LogLevel logLevel)
        {
            //going down from the highest level to the lowest
            //higher tier logs will not be shown
            if (_logLevel >= logLevel)
            {
                Console.WriteLine(LogPrefix() + message);
            }

            return Task.CompletedTask;
        }
    }
}
