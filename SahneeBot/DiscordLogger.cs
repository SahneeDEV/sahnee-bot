using Discord;
using Microsoft.Extensions.Logging;

namespace SahneeBot;

/// <summary>
/// The discord logger is used to translate logs from Discord into an ILogger.
/// </summary>
public class DiscordLogger
{
    private readonly ILogger<DiscordLogger> _logger;

    public DiscordLogger(ILogger<DiscordLogger> logger) => _logger = logger;
    
    /// <summary>
    /// The actual log function.
    /// </summary>
    /// <param name="logMessage">The Discord log message.</param>
    /// <returns>Immediately</returns>
    public Task Log(LogMessage logMessage)
    {
        LogLevel level = LogLevel.None;
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
            {
                level = LogLevel.Critical;
                break;
            }
            case LogSeverity.Debug:
            {
                level = LogLevel.Debug;
                break;
            }
            case LogSeverity.Error:
            {
                level = LogLevel.Error;
                break;
            }
            case LogSeverity.Info:
            {
                level = LogLevel.Information;
                break;
            }
            case LogSeverity.Verbose:
            {
                level = LogLevel.Trace;
                break;
            }
            case LogSeverity.Warning:
            {
                level = LogLevel.Warning;
                break;
            }
        }

        var message = string.IsNullOrEmpty(logMessage.Message) ? logMessage.Exception?.Message : logMessage.Message;
        var source = string.IsNullOrEmpty(logMessage.Source) ?  "" : " (" + logMessage.Source + ")";
        if (logMessage.Exception != null)
        {
            _logger.Log(level, EventIds.Discord, logMessage.Exception, message + source);
        }
        else
        {
            _logger.Log(level, EventIds.Discord, message + source);
        }
        return Task.CompletedTask;
    }
}