using SahneeBot.Formatter.Error;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks.Error;

/// <summary>
/// Reports the error via PM to the guild admins. Returns the amount of users that have been notified.
/// </summary>
public class SahneeBotReportErrorToGuildAdministratorsTask : ITask<SahneeBotReportErrorToGuildAdministratorsTask.Args, ISuccess<uint>>
{
    /// <summary>
    /// Arguments for the task.
    /// </summary>
    /// <param name="InteractionType">The type of interaction.</param>
    /// <param name="InteractionName">The name of the command that was executed.</param>
    /// <param name="FullInteraction">The full command string.</param>
    /// <param name="GuildId">The guild the command was executed on.</param>
    /// <param name="UserId">The user that executed the command.</param>
    /// <param name="Error">The actual error.</param>
    public record struct Args(string InteractionType, string InteractionName, string FullInteraction, ulong GuildId
        , ulong? UserId, ISuccess Error)
    {
        /// <summary>
        /// Creates the args from an error report.
        /// </summary>
        /// <param name="report">The error report.</param>
        /// <param name="opts">The context options.</param>
        /// <returns>The args or null if it cannot be created from the given report.</returns>
        public static Args? FromErrorReport(SahneeBotTaskContextFactory.ErrorReport report
            , SahneeBotTaskContextFactory.ContextOptions opts)
        {
            var (_, error) = report;
            if (error == null || !opts.RelatedGuildId.HasValue)
            {
                return null;
            }
            return new Args(opts.Type, opts.Name
                , opts.Debug, opts.RelatedGuildId.Value, opts.RelatedUserId, error);
        }
    }

    public SahneeBotReportErrorToGuildAdministratorsTask(ErrorDiscordFormatter errorFmt, SahneeBotPrivateMessageToGuildMembersTask privateMessage)
    {
        _errorFmt = errorFmt;
        _privateMessage = privateMessage;
    }

    private readonly ErrorDiscordFormatter _errorFmt;
    private readonly SahneeBotPrivateMessageToGuildMembersTask _privateMessage;

    public async Task<ISuccess<uint>> Execute(ITaskContext ctx, Args arg)
    {
        var (interactionType, interactionName, fullInteraction, guildId, userId
            , success) = arg;
        var message = await _errorFmt.Format(new ErrorDiscordFormatter.Args(interactionType
            , interactionName, fullInteraction,
            guildId, userId, success, false));
        return await _privateMessage.Execute(ctx,
            new SahneeBotPrivateMessageToGuildMembersTask.Args(
                arg.GuildId,
                async guild =>
                {
                    var users = await guild.GetUsersAsync();
                    return users.Where(user => user.GuildPermissions.Administrator);
                },
                new[] {message}));
    }
}