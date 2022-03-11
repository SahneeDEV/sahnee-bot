using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SahneeBot;

/// <summary>
/// Contains release information.
/// </summary>
public class Release
{
    public Release(IConfiguration configuration
        , ILogger<Release> logger)
    {
        var path = configuration["BotSettings:ReleaseInformation"];
        try
        {
            Data = File.ReadAllText(path);
        }
        catch (Exception exception)
        {
            logger.LogWarning(EventIds.Task
                , exception, "Failed to load release information at {Path}"
                , path);
        }

        if (string.IsNullOrEmpty(Data))
        {
            Data = "";
        }
        StartedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// The time the release was started at.
    /// </summary>
    public DateTime StartedAt { get; }
    /// <summary>
    /// The actual release information data.
    /// </summary>
    public string Data { get; }
}