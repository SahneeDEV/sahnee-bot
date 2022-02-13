using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to warn users.
/// </summary>
public class WarnCommand: InteractionModuleBase<IInteractionContext>
{
    private readonly ITaskContext _ctx;
    private readonly GiveWarningToUserTask _task;
    private readonly ILogger<WarnCommand> _logger;
    private readonly WarningDiscordFormatter _discordFormatter;

    public WarnCommand(
        ITaskContext ctx, 
        GiveWarningToUserTask task, 
        ILogger<WarnCommand> logger, 
        WarningDiscordFormatter discordFormatter
    )
    {
        _ctx = ctx;
        _task = task;
        _logger = logger;
        _discordFormatter = discordFormatter;
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
        string reason
        )
    {
        try
        {
            using (_ctx)
            {
                using (var transaction = await _ctx.Model.Database.BeginTransactionAsync())
                {
                    var warning = await _task.Execute(_ctx, new GiveWarningToUserTask.Args(
                        reason, Context.Guild.Id, user.Id, Context.User.Id));
                    try
                    {
                        await _discordFormatter.FormatAndSend(warning, RespondAsync);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(EventIds.Command, e, "Failed to send warning message: {Warning}", 
                            warning);
                    }

                    await transaction.CommitAsync();
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(EventIds.Command, exception, "Error in warn command");
        }
    }
}
