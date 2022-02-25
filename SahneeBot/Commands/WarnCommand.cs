using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBot.Tasks;
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
    private readonly SahneeBotRoleLimitInformationTask _sahneeBotRoleLimitInformationTask;

    public WarnCommand(IServiceProvider serviceProvider, GiveWarningToUserTask task, ILogger<WarnCommand> logger, 
        WarningDiscordFormatter discordFormatter,
        SahneeBotRoleLimitInformationTask sahneeBotRoleLimitInformationTask): base(serviceProvider)
    {
        _task = task;
        _logger = logger;
        _discordFormatter = discordFormatter;
        _sahneeBotRoleLimitInformationTask = sahneeBotRoleLimitInformationTask;
    }
    
    /// <summary>
    /// The actual warn command.
    /// </summary>
    /// <param name="user">The user that is warned.</param>
    /// <param name="reason">The warn reason.</param>
    [SlashCommand("warn", "Warns a user. Adds one to the current warning count")]
    public Task Warn(
        [Discord.Interactions.Summary(description: "the user to warn")]
        IUser user,
        [Discord.Interactions.Summary(description: "the reason why the user was warned")]
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
            await _discordFormatter.FormatAndSend(warning, ModifyOriginalResponseAsync);
            
            //check for role limit
            await _sahneeBotRoleLimitInformationTask.CheckGuildRoleLimit(ctx
                , Context.Interaction as SocketSlashCommand
                , new SahneeBotRoleLimitInformationTask.Args(Context.Guild.Roles.Count, Context.Guild.Id));
        }
        catch (Exception e)
        {
            _logger.LogWarning(EventIds.Command, e, "Failed to send warning message: {Warning}", 
                warning);
        }
    }, new CommandExecutionOptions
    {
        PlaceInQueue = true,
        RequiredRole = RoleType.Moderator
    });
}
