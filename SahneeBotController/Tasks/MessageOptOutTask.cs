namespace SahneeBotController.Tasks;

/// <summary>
/// Opts the given user/guild combination in or out of private messages.
/// </summary>
public class MessageOptOutTask : ITask<MessageOptOutTask.Args, bool>
{
    private readonly GetUserGuildStateTask _guildStateTask;

    /// <summary>
    /// Arguments for this task.
    /// </summary>
    /// <param name="UserId">The user ID that can opt out and should receive the message.</param>
    /// <param name="GuildId">The guild ID the user can opt out of.</param>
    /// <param name="OptOut">Opt out or opt in?</param>
    public record struct Args(ulong UserId, ulong GuildId, bool OptOut);
    
    public MessageOptOutTask(GetUserGuildStateTask guildStateTask)
    {
        _guildStateTask = guildStateTask;
    }

    public async Task<bool> Execute(ITaskContext ctx, Args arg)
    {
        var (userId, guildId, optOut) = arg;
        var state = await _guildStateTask.Execute(ctx, new GetUserGuildStateTask.Args(guildId, userId));
        state.MessageOptOut = optOut;
        await ctx.Model.SaveChangesAsync();
        return state.MessageOptOut;
    }
}