
using Microsoft.Extensions.DependencyInjection;

namespace SahneeBotController.Tasks;

/// <summary>
/// Changes the warning role name of the given guild in the guild state
/// </summary>
public abstract class ChangeWarningRoleNameTask : ITask<ChangeWarningRoleNameTask.Args, ISuccess<string>>
{
    private readonly GetGuildStateTask _getGuildStateTask;

    /// <summary>
    /// Struct
    /// </summary>
    /// <param name="GuildId">the guild id</param>
    /// <param name="WarningRolePrefix">the new warning role prefix</param>
    public record struct Args(ulong GuildId, string WarningRolePrefix);

    protected ChangeWarningRoleNameTask(IServiceProvider provider)
    {
        _getGuildStateTask = provider.GetRequiredService<GetGuildStateTask>();
    }

    public virtual async Task<ISuccess<string>> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, warningRolePrefix) = arg;
        var guildState = await _getGuildStateTask.Execute(ctx, new GetGuildStateTask.Args(guildId));
        // Update the prefix
        if (!warningRolePrefix.EndsWith(" "))
        {
            warningRolePrefix += " ";
        }
        var oldPrefix = guildState.WarningRolePrefix;
        guildState.WarningRolePrefix = warningRolePrefix;
        await ctx.Model.SaveChangesAsync();
        return new Success<string>(oldPrefix);
    }
}
