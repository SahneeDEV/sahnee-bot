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
    private readonly GiveWarningToUserTask _task;
    private readonly ILogger<UnwarnCommand> _logger;
    private readonly WarningDiscordFormatter _discordFormatter;
    private readonly CannotUnwarnDiscordFormatter _cannotUnwarnDiscordFormatter;

    public UnwarnCommand(IServiceProvider serviceProvider,
        GiveWarningToUserTask task,
        ILogger<UnwarnCommand> logger, 
        WarningDiscordFormatter discordFormatter,
        CannotUnwarnDiscordFormatter cannotUnwarnDiscordFormatter): base(serviceProvider)
    {
        _task = task;
        _logger = logger;
        _discordFormatter = discordFormatter;
        _cannotUnwarnDiscordFormatter = cannotUnwarnDiscordFormatter;
    }

    [SlashCommand("unwarn", "Unwarns a user. Removes one from the current warning count")]
    public Task Unwarn(
        [Summary(description: "the user to unwarn")]
        IUser user,
        [Summary(description: "the reason why the user has been unwarned")]
        string reason) => ExecuteAsync(async ctx =>
    {
        var unwarning = await _task.Execute(ctx, new GiveWarningToUserTask.Args(true, Context.Guild.Id, 
            Context.User.Id, user.Id, reason));
        try
        {
            if (unwarning == null)
            {
                await _cannotUnwarnDiscordFormatter.FormatAndSend(user, ModifyOriginalResponseAsync);
            }
            else
            {
                await _discordFormatter.FormatAndSend(unwarning, ModifyOriginalResponseAsync);
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(EventIds.Command, e, "Failed to send unwarning message: {Unwarning}",
                unwarning);
        }
    }, new CommandExecutionOptions(true));
}