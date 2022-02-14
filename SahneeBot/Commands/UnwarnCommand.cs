using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to unwarn users.
/// </summary>
public class UnwarnCommand: CommandBase
{
    private readonly GiveUnwarningToUserTask _task;
    private readonly ILogger<UnwarnCommand> _logger;
    private readonly UnwarningDiscordFormatter _discordFormatter;
    private readonly ModifyUserWarningGroupTask _modifyUserWarningGroupTask;

    public UnwarnCommand(IServiceProvider serviceProvider,
        GiveUnwarningToUserTask task,
        ILogger<UnwarnCommand> logger, 
        UnwarningDiscordFormatter discordFormatter, ModifyUserWarningGroupTask modifyUserWarningGroupTask
        ): base(serviceProvider)
    {
        _task = task;
        _logger = logger;
        _discordFormatter = discordFormatter;
        _modifyUserWarningGroupTask = modifyUserWarningGroupTask;
    }

    [SlashCommand("unwarn", "Unwarns a user. Removes one from the current warning count")]
    public Task Unwarn(
        [Summary(description: "the user to unwarn")]
        IUser user,
        [Summary(description: "the reason why the user has been unwarned")]
        string reason) => ExecuteAsync(async ctx =>
    {
        var unwarning = await _task.Execute(ctx, new GiveUnwarningToUserTask.Args(
            reason, Context.Guild.Id, user.Id, Context.User.Id));
        try
        {
            await _modifyUserWarningGroupTask.Execute(ctx, 
                new ModifyUserWarningGroupTask.Args(unwarning.Number, unwarning.UserId,
                    unwarning.GuildId));
            await _discordFormatter.FormatAndSend(unwarning, ModifyOriginalResponseAsync);
        }
        catch (Exception e)
        {
            _logger.LogWarning(EventIds.Command, e, "Failed to send unwarning message: {Warning}",
                unwarning);
        }
    }, new CommandExecutionOptions(true));
}