namespace SahneeBotController.Tasks;

/// <summary>
/// Changes the channel the given guild is bound to.
/// </summary>
public class ChangeBoundChannelTask : ITask<ChangeBoundChannelTask.Args, ulong?>
{
    /// <summary>
    /// Arguments for setting the bound channel.
    /// </summary>
    /// <param name="GuildId">The guild to set the bound channel of.</param>
    /// <param name="ChannelId">The channel to bind to.</param>
    public record struct Args(ulong GuildId, ulong? ChannelId);
    
    private readonly GetGuildStateTask _getGuildStateTask;
    
    public ChangeBoundChannelTask(GetGuildStateTask getGuildStateTask)
    {
        _getGuildStateTask = getGuildStateTask;
    }

    public async Task<ulong?> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, channelId) = arg;
        var state = await _getGuildStateTask.Execute(ctx, new GetGuildStateTask.Args(guildId));
        state.BoundChannelId = channelId;
        await ctx.Model.SaveChangesAsync();
        return channelId;
    }
}