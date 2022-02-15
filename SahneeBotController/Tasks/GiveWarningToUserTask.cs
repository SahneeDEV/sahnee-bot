using Microsoft.Extensions.Logging;
using SahneeBotModel;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gives the given user a warning.
/// </summary>
public class GiveWarningToUserTask: ITask<GiveWarningToUserTask.Args, IWarning?>
{
    /// <summary>
    /// Arguments for issuing a warning.
    /// </summary>
    public record struct Args(bool Unwarn, ulong GuildId, ulong IssuerUserId, ulong UserId, string Reason);
    
    private readonly ILogger<GiveWarningToUserTask> _logger;
    private readonly GetUserGuildStateTask _getUserGuildStateTask;
    private readonly SendWarningMessageToUserTask _message;
    private readonly ModifyUserWarningGroupTask _modifyUserWarningGroupTask;

    public GiveWarningToUserTask(
        ILogger<GiveWarningToUserTask> logger, GetUserGuildStateTask getUserGuildStateTask,
        SendWarningMessageToUserTask message, ModifyUserWarningGroupTask modifyUserWarningGroupTask)
    {
        _logger = logger;
        _getUserGuildStateTask = getUserGuildStateTask;
        _message = message;
        _modifyUserWarningGroupTask = modifyUserWarningGroupTask;
    }

    public async Task<IWarning?> Execute(ITaskContext ctx, Args arg)
    {
        var userGuildState = await _getUserGuildStateTask.Execute(
            ctx, 
            new GetUserGuildStateTask.Args(arg.GuildId, arg.UserId));
        if (arg.Unwarn && userGuildState.WarningNumber < 1)
        {
            return null;
        }
        var warn = new Warning
        {
            Number = arg.Unwarn ? --userGuildState.WarningNumber : ++userGuildState.WarningNumber,
            Reason = arg.Reason,
            UserId = arg.UserId,
            GuildId = arg.GuildId,
            IssuerUserId = arg.IssuerUserId,
            Type = arg.Unwarn ? WarningType.Unwarning : WarningType.Warning
        };
        _logger.LogDebug(EventIds.Warning, "Issuing warning {Warning}", warn);
        ctx.Model.Warnings.Add(warn);
        await ctx.Model.SaveChangesAsync();
        await _modifyUserWarningGroupTask.Execute(ctx, 
            new ModifyUserWarningGroupTask.Args(warn.Number, warn.UserId,
                warn.GuildId));
        await _message.Execute(ctx, new SendWarningMessageToUserTask.Args(warn, warn.UserId));
        return warn;
    }
}
