using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SahneeBotController.Tasks;

/// <summary>
/// This task sends the user a hint about being able to opt out of messages from a given guild. Returns if the message
/// was sent or not.
/// </summary>
public abstract class SendMessageOptOutHintToUserTask : ITask<SendMessageOptOutHintToUserTask.Args, bool>
{
    private readonly GetUserGuildStateTask _userGuildState;
    private readonly ILogger<SendMessageOptOutHintToUserTask> _logger;

    /// <summary>
    /// Arguments for this task.
    /// </summary>
    /// <param name="UserId">The user ID that can opt out and should receive the message.</param>
    /// <param name="GuildId">The guild ID the user can opt out of.</param>
    public record struct Args(ulong UserId, ulong GuildId);

    protected SendMessageOptOutHintToUserTask(IServiceProvider provider)
    {
        _userGuildState = provider.GetRequiredService<GetUserGuildStateTask>();
        _logger = provider.GetRequiredService<ILogger<SendMessageOptOutHintToUserTask>>();
    }

    public async Task<bool> Execute(ITaskContext ctx, Args arg)
    {
        var (userId, guildId) = arg;
        var state = await _userGuildState.Execute(ctx, new GetUserGuildStateTask.Args(guildId, userId));
        if (state.HasReceivedOptOutHint)
        {
            return false;
        }

        var res = false;
        var ok = true;
        try
        {
            res = await ExecuteImpl(ctx, arg);
        }
        catch (Exception e)
        {
            ok = false;
            _logger.LogWarning(e, "Failed to send opt out hint message to user {UserId} on guild {GuildId}", 
                userId, guildId);
        }

        if (ok)
        {
            state.HasReceivedOptOutHint = true;
            await ctx.Model.SaveChangesAsync();
        }

        return res;
    }

    /// <summary>
    /// Executes this task by sending the actual message. It has already been checked if it actually needs to be sent.
    /// </summary>
    /// <param name="ctx">The task context.</param>
    /// <param name="arg">The user & guild.</param>
    /// <returns></returns>
    protected abstract Task<bool> ExecuteImpl(ITaskContext ctx, Args arg);
}