using Microsoft.Extensions.Configuration;

namespace SahneeBot.Logging;

public class Logger
{
    //Variables
    private LogLevel _logLevel = LogLevel.Warning; //Default logging level

    
    /// <summary>
    /// Creates a new instance of the logger
    /// </summary>
    /// <param name="configuration"></param>
    public Logger(IConfiguration configuration)
    {
        try
        {
            //check if a loglevel has been set
            if (!String.IsNullOrWhiteSpace(configuration["Logging:LogLevel:Default"]))
            {
                _logLevel = Enum.Parse<LogLevel>(configuration["Logging:LogLevel:Default"]);
            }
        }
        catch
        {
            Console.WriteLine("Logging configuration could not be loaded! Reverting to default log level!");
        }
    }

    /// <summary>
    /// Logging with the classname as addition for further easy progressing
    /// </summary>
    /// <param name="message"></param>
    /// <param name="logLevel"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    public Task Log(string message, LogLevel logLevel, string className)
    {
        try
        {
            
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not Log!" + e);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Defines the log suffix that will be appended to every log message
    /// </summary>
    /// <returns></returns>
    private string LogSuffix()
    {
        return "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]: ";
    }

    /// <summary>
    /// Logs to console
    /// </summary>
    /// <param name="message"></param>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    private Task SendLogToConsole(string message, LogLevel logLevel)
    {
        //check if the loglevel of the log message is equal or higher than the current loglevel
        
        return Task.CompletedTask;
    }
    
}
