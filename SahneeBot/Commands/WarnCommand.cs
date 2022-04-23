using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SahneeBot.Formatter;
using SahneeBot.Tasks;
using SahneeBotController;
using SahneeBotController.Tasks;
using SahneeBotModel;
using SahneeBotModel.Contract;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to warn users.
/// </summary>
public class WarnCommand : CommandBase
{
    private readonly GiveWarningToUserTask _task;
    private readonly WarningDiscordFormatter _fmt;
    private readonly FailedToWarnDiscordFormatter _failedToWarnFmt;

    public WarnCommand(IServiceProvider serviceProvider
        , GiveWarningToUserTask task
        , WarningDiscordFormatter fmt
        , FailedToWarnDiscordFormatter failedToWarnFmt): base(serviceProvider)
    {
        _task = task;
        _fmt = fmt;
        _failedToWarnFmt = failedToWarnFmt;
    }
    
    /// <summary>
    /// The actual warn command.
    /// </summary>
    /// <param name="user">The user that is warned.</param>
    /// <param name="reason">The warn reason.</param>
    /// <returns>Once the warning has been issued</returns>
    [SlashCommand("warn", "Warns a user. Adds one to the current warning count")]
    public Task Warn(
        [Summary(description: "the user to warn")]
        IUser user,
        [Summary(description: "the reason why the user was warned")]
        string reason) => ExecuteAsync(async ctx => await HelperIssueWarning(ctx, WarningType.Warning
        , Context.Guild, Context.User, user, reason), new CommandExecutionOptions
    {
        PlaceInQueue = true,
        RequiredRole = RoleType.Moderator
    });

    /// <summary>
    /// Unwarns a user
    /// </summary>
    /// <param name="user">The user to unwarn</param>
    /// <param name="reason">The unwarning reason</param>
    /// <returns>Once the unwarning has been issued</returns>
    [SlashCommand("unwarn", "Unwarns a user. Removes one from the current warning count")]
    public Task Unwarn(
        [Summary(description: "the user to unwarn")]
        IUser user,
        [Summary(description: "the reason why the user has been unwarned")]
        string reason) => ExecuteAsync(async ctx => await HelperIssueWarning(ctx, WarningType.Unwarning
        , Context.Guild, Context.User, user, reason), new CommandExecutionOptions
    {
        PlaceInQueue = true,
        RequiredRole = RoleType.Moderator
    });

    private async Task<ISuccess<IWarning>> HelperIssueWarning(ITaskContext ctx, WarningType type, IGuild guild
        , IUser issuer, IUser user, string reason)
    {
        var warning = await _task.Execute(ctx, new GiveWarningToUserTask.Args(type, guild.Id
            , issuer.Id, user.Id, reason));
        if (warning.IsSuccess)
        {
            await _fmt.FormatAndSend(warning.Value, ModifyOriginalResponseAsync);
        }

        return warning;
    }
}
