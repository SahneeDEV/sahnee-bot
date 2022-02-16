using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gets a random warning. Returns null if no warning exists.
/// </summary>
public class GetRandomWarningTask : ITask<GetRandomWarningTask.Args, IWarning?>
{
    /// <summary>
    /// Arguments to get the warning.
    /// </summary>
    /// <param name="GuildId">The guild to get the warning in.</param>
    /// <param name="UserId">If specified the warning must be of the given user.</param>
    public record struct Args(ulong GuildId, ulong? UserId);
    
    public async Task<IWarning?> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, userId) = arg;
        var count = await ctx.Model.Warnings.CountAsync();
        var rnd = new Random();
        var toSkip = rnd.Next(0, count);
        return await ctx.Model.Warnings
            .Skip(toSkip)
            .Where(w => w.GuildId == guildId && (!userId.HasValue || w.UserId == userId.Value))
            .FirstOrDefaultAsync<IWarning>();
    }
}