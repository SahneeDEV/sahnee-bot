using Discord;
using Microsoft.Extensions.Logging;

namespace SahneeBot;

public class DiscordLogger
{
    private readonly ILogger<DiscordLogger> _logger;

    public DiscordLogger(ILogger<DiscordLogger> logger) => _logger = logger;
    
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
        if (logMessage.Exception != null)
        {
            _logger.Log(level, EventIds.Discord, logMessage.Exception, message + " (" + logMessage.Source +  ")");
        }
        else
        {
            _logger.Log(level, EventIds.Discord, message + " (" + logMessage.Source +  ")");
        }
        return Task.CompletedTask;
    }
}