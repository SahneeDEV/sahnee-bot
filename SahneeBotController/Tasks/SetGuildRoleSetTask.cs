using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

public class SetGuildRoleSetTask : ITask<SetGuildRoleSetTask.Args, IGuildState>
{
    private readonly GetGuildStateTask _getGuildStateTask;

    /// <summary>
    /// Arguments for setting the role set state
    /// </summary>
    public struct Args
    {
        /// <summary>
        /// The id of the guild
        /// </summary>
        public readonly ulong GuildId;

        /// <summary>
        /// True for enabled and False for disabled
        /// </summary>
        public readonly bool Enable;

        public Args(ulong guildId, bool enable)
        {
            GuildId = guildId;
            Enable = enable;
        }
    }

    public SetGuildRoleSetTask(GetGuildStateTask getGuildStateTask)
    {
        _getGuildStateTask = getGuildStateTask;
    }

    public async Task<IGuildState> Execute(ITaskContext ctx, Args args)
    {
        var guildState = await _getGuildStateTask.Execute(ctx, new GetGuildStateTask.Args(args.GuildId));
        switch (args.Enable)
        {
            case true:
                guildState.SetRoles = true;
                break;
            case false:
                guildState.SetRoles = false;
                break;
        }
        await ctx.Model.SaveChangesAsync();
        return guildState;
    }
}