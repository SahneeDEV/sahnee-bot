using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBot.Jobs.JobTasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

public class CleanupWarningRolesCommand : CommandBase
{
    private readonly ILogger<CleanupWarningRolesCommand> _logger;
    private readonly CleanupWarningRolesJob _cleanupWarningRolesJob;
    private readonly WarningRoleCleanupDiscordFormatter _warningRoleCleanupDiscordFormatter;

    public CleanupWarningRolesCommand(IServiceProvider serviceProvider, ILogger<CleanupWarningRolesCommand> logger
    , CleanupWarningRolesJob cleanupWarningRolesJob
    , WarningRoleCleanupDiscordFormatter warningRoleCleanupDiscordFormatter) : base(serviceProvider)
    {
        _logger = logger;
        _cleanupWarningRolesJob = cleanupWarningRolesJob;
        _warningRoleCleanupDiscordFormatter = warningRoleCleanupDiscordFormatter;
    }

    [SlashCommand("cleanup-roles", "Removes all unused warning roles from your server.")]
    public Task CleanupRoles() => ExecuteAsync(async ctx =>
    {
        try
        {
            var amount = await _cleanupWarningRolesJob
                .CleanupWarningRolesForAGuild(Context.Guild as SocketGuild, ctx.Model);
            await _warningRoleCleanupDiscordFormatter.FormatAndSend(
                new WarningRoleCleanupDiscordFormatter.Args(amount), ModifyOriginalResponseAsync);
        }
        catch (Exception e)
        {
            _logger.LogWarning(EventIds.Command, e, "Failed to manually removes roles from the" +
                                                    " guild: {guildId}", Context.Guild.Id);
        }
    },
    new CommandExecutionOptions
    {
        PlaceInQueue = true,
        RequiredRole = RoleType.Administrator
    });
}
