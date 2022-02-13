using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to unwarn users.
/// </summary>
public class UnwarnCommand: InteractionModuleBase<IInteractionContext>
{
    private readonly ITaskContext _ctx;
    private readonly GiveUnwarningToUserTask _task;
    private readonly ILogger<UnwarnCommand> _logger;
    private readonly UnwarningDiscordFormatter _discordFormatter;

    public UnwarnCommand(ITaskContext ctx, 
        GiveUnwarningToUserTask task,
        ILogger<UnwarnCommand> logger, 
        UnwarningDiscordFormatter discordFormatter)
    {
        _ctx = ctx;
        _task = task;
        _logger = logger;
        _discordFormatter = discordFormatter;
    }

    [SlashCommand("unwarn", "Unwarns a user. Removes one from the current warning count")]
    public async Task Unwarn(
        [Summary(description: "the user to unwarn")]
        IUser user,
        [Summary(description: "the reason why the user has been unwarned")]
        string reason)
    {
        try
        {
            using (_ctx)
            {
                using (var transaction = await _ctx.Model.Database.BeginTransactionAsync())
                {
                    var unwarning = await _task.Execute(_ctx, new GiveUnwarningToUserTask.Args(
                        reason, Context.Guild.Id, user.Id, Context.User.Id));
                    try
                    {
                        await _discordFormatter.FormatAndSend(unwarning, RespondAsync);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(EventIds.Command, e, "Failed to send unwarning message: {Warning}", 
                            unwarning);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(EventIds.Command, exception, "Error in unwarn command");
        }
    }
}