using Microsoft.Extensions.DependencyInjection;

namespace SahneeBotController.Tasks;

/// <summary>
/// Checks if the guild role limit got hit, and will notify the guild if so.
/// </summary>
public abstract class CheckRoleLimitTask : ITask<CheckRoleLimitTask.Args, bool>
{
    /// <summary>
    /// The service provider.
    /// </summary>
    protected IServiceProvider Provider { get; }
    
    private const int ROLE_LIMIT_THRESHOLD = 245;
    private readonly GetGuildStateTask _getGuildStateTask;

    /// <summary>
    /// Task arguments.
    /// </summary>
    /// <param name="GuildId">The guild to check.</param>
    public record struct Args(ulong GuildId);

    protected CheckRoleLimitTask(IServiceProvider provider)
    {
        Provider = provider;
        _getGuildStateTask = provider.GetRequiredService<GetGuildStateTask>();
    }

    public async Task<bool> Execute(ITaskContext ctx, Args arg)
    {
        var guildId = arg.GuildId;
        
        // Role handling disabled
        var guildState = await _getGuildStateTask.Execute(ctx, new GetGuildStateTask.Args(guildId));
        if (!guildState.SetRoles)
        {
            return false;
        }
        
        // Still enough role slots
        var roleCount = await GetRoleCount(ctx, guildId);
        if (roleCount <= ROLE_LIMIT_THRESHOLD)
        {
            return false;
        }

        await SendWarning(ctx, guildId, roleCount);
        return true;
    }

    /// <summary>
    /// Gets the role count on the given guild.
    /// </summary>
    /// <param name="ctx">The context.</param>
    /// <param name="guildId">The guild ID.</param>
    /// <returns>The amount of roles.</returns>
    protected abstract Task<uint> GetRoleCount(ITaskContext ctx, ulong guildId);
    /// <summary>
    /// Informs the guild admins about the role count.
    /// </summary>
    /// <param name="ctx">The context.</param>
    /// <param name="guildId">The guild ID.</param>
    /// <param name="roleCount">The role count.</param>
    /// <returns>Once the admins have been informed.</returns>
    protected abstract Task SendWarning(ITaskContext ctx, ulong guildId, uint roleCount);
}