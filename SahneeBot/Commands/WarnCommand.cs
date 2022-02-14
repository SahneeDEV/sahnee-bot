using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to warn users.
/// </summary>
public class WarnCommand: CommandBase
{
    private readonly GiveWarningToUserTask _task;
    private readonly ILogger<WarnCommand> _logger;
    private readonly WarningDiscordFormatter _discordFormatter;
    private readonly ModifyUserWarningGroupTask _modifyUserWarningGroupTask;

    public WarnCommand(IServiceProvider serviceProvider, GiveWarningToUserTask task, ILogger<WarnCommand> logger, 
        WarningDiscordFormatter discordFormatter,
        ModifyUserWarningGroupTask modifyUserWarningGroupTask): base(serviceProvider)
    {
        _task = task;
        _logger = logger;
        _discordFormatter = discordFormatter;
        _modifyUserWarningGroupTask = modifyUserWarningGroupTask;
    }
    
    /// <summary>
    /// The actual warn command.
    /// </summary>
    /// <param name="user">The user that is warned.</param>
    /// <param name="reason">The warn reason.</param>
    [SlashCommand("warn", "Warns a user. Adds one to the current warning count")]
    public Task Warn(
        [Summary(description: "the user to warn")]
        IUser user,
        [Summary(description: "the reason why the user was warned")]
        string reason
        ) => ExecuteAsync(async ctx => 
    {
        var warning = await _task.Execute(ctx, new GiveWarningToUserTask.Args(false, Context.Guild.Id, 
            Context.User.Id, user.Id, reason));
        try
        {
            if (warning == null)
            {
                throw new Exception("Warning is null");
            }
            await _modifyUserWarningGroupTask.Execute(ctx, 
                new ModifyUserWarningGroupTask.Args(warning.Number, warning.UserId,
                    warning.GuildId));
            await _discordFormatter.FormatAndSend(warning, ModifyOriginalResponseAsync);
        }
        catch (Exception e)
        {
            _logger.LogWarning(EventIds.Command, e, "Failed to send warning message: {Warning}", 
                warning);
        }
    }, new CommandExecutionOptions
    {
        PlaceInQueue = true,
        RequiredRole = RoleTypes.Moderator
    });
}
