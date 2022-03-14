using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SahneeBot.Tasks;

namespace SahneeBot.Jobs;

/// <summary>
/// The cleanup warning job regularly cleans all warnings roles in the system that are no longer used.
/// </summary>

[Job]
public sealed class CleanupWarningRolesJob : JobBase
{
    private readonly ILogger<CleanupWarningRolesJob> _logger;
    private readonly Bot _bot;
    private readonly SahneeBotCleanupWarningRolesTask _cleanupWarningRolesTask;

    public CleanupWarningRolesJob(IServiceProvider provider
        , IConfiguration cfg
        , ILogger<CleanupWarningRolesJob> logger
        , Bot bot
        , SahneeBotCleanupWarningRolesTask cleanupWarningRolesTask) : base(provider)
    {
        _logger = logger;
        _bot = bot;
        _cleanupWarningRolesTask = cleanupWarningRolesTask;
        Time = new JobTimeSpanRepeat(TimeSpan.Parse(cfg["BotSettings:Jobs:CleanupWarningRoles"]));
    }

    public override async Task Perform()
    {
        _logger.LogDebug(EventIds.Jobs, "Starting Role deletion on Guilds");
        foreach (var currentGuild in await _bot.Client.GetGuildsAsync())
        {
            var guild = currentGuild;
            if (guild == null)
            {
                continue;
            }

            await PerformAsync(async ctx => await _cleanupWarningRolesTask.Execute(ctx
                , new SahneeBotCleanupWarningRolesTask.Args(guild.Id)), new JobExecutionOptions
            {
                PlaceInQueue = guild.Id
                , RelatedGuildId = guild.Id
                , Name = "warning role cleanup"
                , Debug = guild.Id.ToString()
            });
        }
    }
}
