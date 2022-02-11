using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SahneeBotController.Tasks;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to warn users.
/// </summary>
public class WarnCommand: InteractionModuleBase<IInteractionContext>
{
    private readonly GiveWarningToUserTask _task;
    private readonly ILogger<WarnCommand> _logger;

    public WarnCommand(GiveWarningToUserTask task, ILogger<WarnCommand> logger)
    {
        _task = task;
        _logger = logger;
    }
    
    /// <summary>
    /// The actual warn command.
    /// </summary>
    /// <param name="user">The user that is warned.</param>
    /// <param name="reason">The warn reason.</param>
    [SlashCommand("warn", "Warns a user")]
    public async Task Warn(
        [Summary(description: "the user to warn")]
        IUser user,
        [Summary(description: "the reason why the user was warned")]
        string reason = ""
        )
    {
        try
        {
            await _task.Execute(Context.Guild.Id, Context.User.Id, user.Id, reason);
            await RespondAsync(reason);
        }
        catch (Exception exception)
        {
            _logger.LogError(EventIds.Command, exception, "Error in warn command");
        }
    }
}
