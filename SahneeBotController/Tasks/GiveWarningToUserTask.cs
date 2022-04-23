using Microsoft.Extensions.Logging;
using SahneeBotModel;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gives the given user a warning.
/// </summary>
public class GiveWarningToUserTask: ITask<GiveWarningToUserTask.Args, ISuccess<IWarning>>
{
    /// <summary>
    /// Arguments for issuing a warning.
    /// </summary>
    public record struct Args(WarningType Type, ulong GuildId, ulong IssuerUserId, ulong UserId, string Reason);
    
    private readonly ILogger<GiveWarningToUserTask> _logger;
    private readonly GetUserGuildStateTask _getUserGuildStateTask;
    private readonly SendWarningMessageToUserTask _message;
    private readonly ModifyUserWarningRoleTask _modifyUserWarningRoleTask;

    public GiveWarningToUserTask(
        ILogger<GiveWarningToUserTask> logger, GetUserGuildStateTask getUserGuildStateTask,
        SendWarningMessageToUserTask message, ModifyUserWarningRoleTask modifyUserWarningRoleTask)
    {
        _logger = logger;
        _getUserGuildStateTask = getUserGuildStateTask;
        _message = message;
        _modifyUserWarningRoleTask = modifyUserWarningRoleTask;
    }

    public async Task<ISuccess<IWarning>> Execute(ITaskContext ctx, Args arg)
    {
        var userGuildState = await _getUserGuildStateTask.Execute(
            ctx, 
            new GetUserGuildStateTask.Args(arg.GuildId, arg.UserId));
        if (arg.Type == WarningType.Unwarning && userGuildState.WarningNumber < 1)
        {
            return new Error<IWarning>("Cannot unwarn users with no warnings.");
        }
        var warn = new Warning
        {
            Number = arg.Type == WarningType.Unwarning 
                ? --userGuildState.WarningNumber 
                : ++userGuildState.WarningNumber
            , Reason = arg.Reason
            , UserId = arg.UserId
            , GuildId = arg.GuildId
            , IssuerUserId = arg.IssuerUserId
            , Type = arg.Type
        };
        _logger.LogDebug(EventIds.Warning, "Issuing warning {Warning}", warn);
        ctx.Model.Warnings.Add(warn);
        await ctx.Model.SaveChangesAsync();
        var groupSuccess = await _modifyUserWarningRoleTask.Execute(ctx, 
            new ModifyUserWarningRoleTask.Args(warn.Number, warn.UserId,
                warn.GuildId));
        if (!groupSuccess.IsSuccess)
        {
            _logger.LogWarning(EventIds.Warning
                , "Failed to assign warning group, erroring warning {Warning}: {Error}"
                , warn, groupSuccess.Message);
            return new Error<IWarning>(groupSuccess.Message);
        }
        await _message.Execute(ctx, new SendWarningMessageToUserTask.Args(warn));
        return new Success<IWarning>(warn);
    }
}
