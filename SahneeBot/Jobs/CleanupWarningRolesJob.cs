using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBot.Tasks;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Jobs;

/// <summary>
/// The cleanup warning job regularly cleans all warnings roles in the system that are no longer used.
/// </summary>
public class CleanupWarningRolesJob : JobBase
{
    private readonly ILogger<CleanupWarningRolesJob> _logger;
    private readonly Bot _bot;
    private readonly SahneeBotPrivateMessageToGuildMembersTask _privateMessage;
    private readonly SahneeBotCleanupWarningRolesTask _cleanupWarningRolesTask;
    private readonly JobFailedDiscordFormatter _jobFailedDiscordFormatter;

    public CleanupWarningRolesJob(IServiceProvider provider
        , ILogger<CleanupWarningRolesJob> logger
        , Bot bot
        , SahneeBotPrivateMessageToGuildMembersTask privateMessage
        , SahneeBotCleanupWarningRolesTask cleanupWarningRolesTask
        , JobFailedDiscordFormatter jobFailedDiscordFormatter) : base(provider)
    {
        _logger = logger;
        _bot = bot;
        _privateMessage = privateMessage;
        _cleanupWarningRolesTask = cleanupWarningRolesTask;
        _jobFailedDiscordFormatter = jobFailedDiscordFormatter;
    }

    /// <summary>
    /// Cleans all warning roles for a single guild
    /// </summary>
    private async Task<ISuccess<uint>> CleanupWarningRolesForAGuild(ITaskContext ctx, IGuild currentGuild)
    {
        var cleanedSuccess = await _cleanupWarningRolesTask.Execute(ctx
            , new SahneeBotCleanupWarningRolesTask.Args(currentGuild.Id));
        if (cleanedSuccess.IsSuccess)
        {
            return cleanedSuccess;
        }
        var message = await _jobFailedDiscordFormatter.Format(new JobFailedDiscordFormatter.Args(
            currentGuild.Id, "warning roles cleanup", cleanedSuccess.Message));
        var pmSuccess = await _privateMessage.Execute(ctx, 
            new SahneeBotPrivateMessageToGuildMembersTask.Args(
                currentGuild.Id,
                async guild =>
                {
                    var users = await guild.GetUsersAsync();
                    return users.Where(user => user.GuildPermissions.Administrator);
                },
                new [] {message}));
        // If the PM failed, return that error instead
        return pmSuccess.IsSuccess ? cleanedSuccess : pmSuccess;
    }

    public override Task Perform() => PerformAsync(async ctx =>
    {
        _logger.LogDebug(EventIds.Jobs, "Starting Role deletion on Guilds");
        foreach (var currentGuild in await _bot.Client.GetGuildsAsync())
        {
            if (currentGuild == null)
            {
                continue;
            }
            var cleanSuccess = await CleanupWarningRolesForAGuild(ctx, currentGuild);
            if (!cleanSuccess.IsSuccess)
            {
                _logger.LogWarning(EventIds.Jobs
                    , "Warning role cleanup job failed for guild {Guild}: {Error}"
                    , currentGuild.Id, cleanSuccess.Message);
            }
        }
    });
}
