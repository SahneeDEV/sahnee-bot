
namespace SahneeBotController.Tasks;

/// <summary>
/// Changes to role color for a specific guild
/// </summary>
public class ChangeWarningRoleNameTask : ITask<ChangeWarningRoleNameTask.Args, string>
{
    private readonly GetGuildStateTask _getGuildStateTask;

    /// <summary>
    /// Struct
    /// </summary>
    /// <param name="GuildId">the guild id</param>
    /// <param name="WarningRolePrefix">the new warning role prefix</param>
    public record struct Args(ulong GuildId, string WarningRolePrefix);

    public ChangeWarningRoleNameTask(GetGuildStateTask getGuildStateTask)
    {
        _getGuildStateTask = getGuildStateTask;
    }

    public async Task<string> Execute(ITaskContext ctx, Args arg)
    {
        var guildState = await _getGuildStateTask.Execute(ctx, new GetGuildStateTask.Args(arg.GuildId));
        //update the prefix
        if (!arg.WarningRolePrefix.EndsWith(" "))
        {
            arg.WarningRolePrefix += " ";
        }
        var oldPrefix = guildState.WarningRolePrefix;
        guildState.WarningRolePrefix = arg.WarningRolePrefix;
        await ctx.Model.SaveChangesAsync();
        return oldPrefix;
    }
}
