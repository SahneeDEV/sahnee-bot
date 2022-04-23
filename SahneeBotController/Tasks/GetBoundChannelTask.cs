namespace SahneeBotController.Tasks;

/// <summary>
/// This task gets the ID of the bound channel for the given guild. (if any)
/// </summary>
public class GetBoundChannelTask : ITask<GetBoundChannelTask.Args, ulong?>
{
    /// <summary>
    /// Arguments for getting the bound channel.
    /// </summary>
    /// <param name="GuildId">The guild ID to get the bound channel of.</param>
    public record struct Args(ulong GuildId);
    
    private readonly GetGuildStateTask _guildStateTask;

    public GetBoundChannelTask(GetGuildStateTask guildStateTask) => _guildStateTask = guildStateTask;
    
    public async Task<ulong?> Execute(ITaskContext ctx, Args arg)
    {
        var state = await _guildStateTask.Execute(ctx, new GetGuildStateTask.Args(arg.GuildId));
        return state.BoundChannelId;
    }
}