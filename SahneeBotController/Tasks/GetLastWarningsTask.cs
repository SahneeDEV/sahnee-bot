using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gets the last X warnings of a guild.
/// </summary>
public class GetLastWarningsTask : ITask<GetLastWarningsTask.Args, IEnumerable<IWarning>>
{
    /// <summary>
    /// Arguments for getting the last warnings.
    /// </summary>
    /// <param name="GuildId">The guild ID to get the warnings of.</param>
    /// <param name="UserId">The user ID to get the warnings of.</param>
    /// <param name="Issuer">The user issued the warning.</param>
    /// <param name="MaxAmount">The max amount of warnings to get.</param>
    public record struct Args(ulong GuildId, ulong? UserId, bool Issuer, int MaxAmount);

    public async Task<IEnumerable<IWarning>> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, userId, warner, maxAmount) = arg;
        var list = await ctx.Model.Warnings
            .Where(w => w.GuildId == guildId && (!userId.HasValue || warner 
                ? w.IssuerUserId == userId 
                : w.UserId == userId))
            .OrderByDescending(w => w.Time)
            .Take(maxAmount)
            .ToListAsync<IWarning>();
        return list;
    }
}