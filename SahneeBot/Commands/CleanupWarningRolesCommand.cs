using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBot.Tasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

/// <summary>
/// This command manually triggers a cleanup of all unused warning roles in a guild.
/// </summary>
public class CleanupWarningRolesCommand : CommandBase
{
    private readonly SahneeBotCleanupWarningRolesTask _cleanupWarningRolesTask;
    private readonly RoleCleanupFailedDiscordFormatter _errorFmt;
    private readonly ILogger<CleanupWarningRolesCommand> _logger;
    private readonly WarningRoleCleanupDiscordFormatter _warningRoleCleanupDiscordFormatter;

    public CleanupWarningRolesCommand(IServiceProvider serviceProvider
        , ILogger<CleanupWarningRolesCommand> logger
        , WarningRoleCleanupDiscordFormatter warningRoleCleanupDiscordFormatter
        , SahneeBotCleanupWarningRolesTask cleanupWarningRolesTask
        , RoleCleanupFailedDiscordFormatter errorFmt) : base(serviceProvider)
    {
        _logger = logger;
        _warningRoleCleanupDiscordFormatter = warningRoleCleanupDiscordFormatter;
        _cleanupWarningRolesTask = cleanupWarningRolesTask;
        _errorFmt = errorFmt;
    }

    [SlashCommand("cleanup-roles", "Removes all unused warning roles from your server.")]
    public Task CleanupRoles() => ExecuteAsync(async ctx =>
        {
            var cleanupSuccess = await _cleanupWarningRolesTask.Execute(ctx
                , new SahneeBotCleanupWarningRolesTask.Args(Context.Guild.Id));
            if (cleanupSuccess.IsSuccess)
            {
                await _warningRoleCleanupDiscordFormatter.FormatAndSend(
                    new WarningRoleCleanupDiscordFormatter.Args((int) cleanupSuccess.Value)
                    , ModifyOriginalResponseAsync);
            }
            else
            {
                _logger.LogWarning(EventIds.Command
                    , "Failed to cleanup warnings via command on guild {Guild}: {Error}"
                    , Context.Guild.Id, cleanupSuccess.Message);
                await _errorFmt.FormatAndSend(new RoleCleanupFailedDiscordFormatter.Args(cleanupSuccess.Message)
                    , ModifyOriginalResponseAsync);
            }

            return cleanupSuccess;
        },
        new CommandExecutionOptions
        {
            PlaceInQueue = true
            , RequiredRole = RoleType.Administrator
        });
}