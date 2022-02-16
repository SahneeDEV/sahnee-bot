using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gets a random warning. Returns null if no warning exists.
/// </summary>
public class GetRandomWarningsTask : ITask<GetRandomWarningsTask.Args, IEnumerable<IWarning>>
{
    /// <summary>
    /// Arguments to get the warning.
    /// </summary>
    /// <param name="GuildId">The guild to get the warning in.</param>
    /// <param name="UserId">If specified the warning must be of the given user.</param>
    public record struct Args(ulong GuildId, ulong? UserId, int Count);
    
    public async Task<IEnumerable<IWarning>> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, userId, count) = arg;
        var total = await ctx.Model.Warnings.CountAsync(w => w.GuildId == guildId 
                                                             && (!userId.HasValue || w.UserId == userId.Value));
        var rnd = new Random();
        var toSkip = rnd.Next(0, Math.Max(0, total - count));
        return await ctx.Model.Warnings
            .Where(w => w.GuildId == guildId && (!userId.HasValue || w.UserId == userId.Value))
            .Skip(toSkip)
            .Take(count)
            .ToListAsync<IWarning>();
    }
}