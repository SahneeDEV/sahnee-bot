using Microsoft.Extensions.Logging;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

public class GiveUnwarningToUserTask: ITask<GiveUnwarningToUserTask.Args, IWarning>
{
    private readonly ILogger<GiveUnwarningToUserTask> _logger;
    private readonly GetUserGuildStateTask _getUserGuildStateTask;
    private readonly SendUnwarningMessageToUserTask _message;

    /// <summary>
    /// Arguments for issuing a unwarning.
    /// </summary>
    public struct Args
    {
        /// <summary>
        /// The guild ID the unwarning was issued on.
        /// </summary>
        public readonly ulong GuildId;
        /// <summary>
        /// The user ID that issued the unwarning.
        /// </summary>
        public readonly ulong IssuerUserId;
        /// <summary>
        /// The user ID that received the unwarning.
        /// </summary>
        public readonly ulong UserId;
        /// <summary>
        /// The reason of the unwarning.
        /// </summary>
        public readonly string Reason;

        public Args(string reason, ulong guildId, ulong userId, ulong issuerUserId)
        {
            Reason = reason;
            GuildId = guildId;
            UserId = userId;
            IssuerUserId = issuerUserId;
        }
    }

    public GiveUnwarningToUserTask(
        ILogger<GiveUnwarningToUserTask> logger, GetUserGuildStateTask getUserGuildStateTask,
        SendUnwarningMessageToUserTask message)
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
            Number = --userGuildState.WarningNumber,
            Reason = args.Reason,
            UserId = args.UserId,
            GuildId = args.GuildId,
            IssuerUserId = args.IssuerUserId
        };
        _logger.LogDebug(EventIds.Warning, "Issuing warning {warning}", warn);
        ctx.Model.Warnings.Add(warn);
        await ctx.Model.SaveChangesAsync();
        await _message.Execute(ctx, new SendUnwarningMessageToUserTask.Args(warn));
        return warn;
    }
}
