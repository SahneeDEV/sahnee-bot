using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// This task gets the state of a specific guild or creates it if it does not exist.
/// </summary>
public class GetGuildStateTask: ITask<GetGuildStateTask.Args, IGuildState>
{
    /// <summary>
    /// Arguments for getting the guild state.
    /// </summary>
    public struct Args
    {
        /// <summary>
        /// The guild ID of the user.
        /// </summary>
        public readonly ulong GuildId;

        public Args(ulong guildId)
        {
            GuildId = guildId;
        }
    }

    public async Task<IGuildState> Execute(ITaskContext ctx, Args args)
    {
        var guildState = await ctx.Model.GuildStates
            .FirstOrDefaultAsync(g => g.GuildId == args.GuildId);
        if (guildState != null)
        {
            return guildState;
        }
        guildState = new GuildState
        {
            GuildId = args.GuildId
        };
        ctx.Model.GuildStates.Add(guildState);
        await ctx.Model.SaveChangesAsync();
        return guildState;
    }
}
