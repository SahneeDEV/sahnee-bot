using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBot.Tasks;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to unwarn users.
/// </summary>
public class UnwarnCommand: CommandBase
{
    private readonly GiveWarningToUserTask _task;
    private readonly ILogger<UnwarnCommand> _logger;
    private readonly ModifyUserWarningGroupTask _modifyUserWarningGroupTask;
    private readonly WarningDiscordFormatter _discordFormatter;
    private readonly CannotUnwarnDiscordFormatter _cannotUnwarnDiscordFormatter;
    private readonly SahneeBotRoleLimitInformationTask _sahneeBotRoleLimitInformationTask;

    public UnwarnCommand(IServiceProvider serviceProvider,
        GiveWarningToUserTask task,
        ILogger<UnwarnCommand> logger, 
        WarningDiscordFormatter discordFormatter,
        ModifyUserWarningGroupTask modifyUserWarningGroupTask,
        CannotUnwarnDiscordFormatter cannotUnwarnDiscordFormatter,
        SahneeBotRoleLimitInformationTask sahneeBotRoleLimitInformationTask
    ): base(serviceProvider)
    {
        _task = task;
        _logger = logger;
        _discordFormatter = discordFormatter;
        _modifyUserWarningGroupTask = modifyUserWarningGroupTask;
        _cannotUnwarnDiscordFormatter = cannotUnwarnDiscordFormatter;
        _sahneeBotRoleLimitInformationTask = sahneeBotRoleLimitInformationTask;
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
                await _modifyUserWarningGroupTask.Execute(ctx, 
                    new ModifyUserWarningGroupTask.Args(unwarning.Number, unwarning.UserId,
                        unwarning.GuildId));
                await _discordFormatter.FormatAndSend(unwarning, ModifyOriginalResponseAsync);
            
                //check for role limit
                await _sahneeBotRoleLimitInformationTask.CheckGuildRoleLimit(ctx
                    , Context.Interaction as SocketSlashCommand
                    , new SahneeBotRoleLimitInformationTask.Args(Context.Guild.Roles.Count, Context.Guild.Id));
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(EventIds.Command, e, "Failed to send unwarning message: {Unwarning}",
                unwarning);
        }
    }, new CommandExecutionOptions
    {
        PlaceInQueue = true,
        RequiredRole = RoleType.Moderator
    });
}