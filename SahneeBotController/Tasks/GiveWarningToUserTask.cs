using Microsoft.Extensions.Logging;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gives the given user a warning.
/// </summary>
public class GiveWarningToUserTask: ITask<GiveWarningToUserTask.Args, IWarning>
{
    public class Args
    {
        public readonly ulong GuildId;
        public readonly ulong IssuerUserId;
        public readonly ulong UserId;
        public readonly string Reason;

        public Args(string reason, ulong guildId, ulong userId, ulong issuerUserId)
        {
            Reason = reason;
            GuildId = guildId;
            UserId = userId;
            IssuerUserId = issuerUserId;
        }
    }
    
    private readonly ILogger<GiveWarningToUserTask> _logger;
    private readonly GetUserGuildStateTask _getUserGuildStateTask;
    private readonly SendWarningMessageToUserTask _message;

    public GiveWarningToUserTask(
        ILogger<GiveWarningToUserTask> logger, GetUserGuildStateTask getUserGuildStateTask,
        SendWarningMessageToUserTask message)
    {
        _logger = logger;
        _getUserGuildStateTask = getUserGuildStateTask;
        _message = message;
    }

    public async Task<IWarning> Execute(ITaskContext ctx, Args args)
    {
        var userGuildState = await _getUserGuildStateTask.Execute(
            ctx, 
            new GetUserGuildStateTask.Args(args.GuildId, args.UserId));
        var warn = new Warning
        {
            Number = ++userGuildState.WarningNumber,
            Reason = args.Reason,
            UserId = args.UserId,
            GuildId = args.GuildId,
            IssuerUserId = args.IssuerUserId
        };
        _logger.LogDebug("Issuing warning {warning}", warn);
        ctx.Model.Warnings.Add(warn);
        await _message.Execute(ctx, new SendWarningMessageToUserTask.Args(warn));
        return warn;
    }
}
