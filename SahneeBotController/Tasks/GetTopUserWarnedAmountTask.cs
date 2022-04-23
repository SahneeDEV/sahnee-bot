using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace SahneeBotController.Tasks;

/// <summary>
/// Returns the most warned users sorted by their warnings
/// </summary>
public class GetTopUserWarnedAmountTask: ITask<
    GetTopUserWarnedAmountTask.Args, 
    IEnumerable<GetTopUserWarnedAmountTask.ReturnValue>
>
{
    public record struct Args(int MaxRankings, ulong GuildId);
    
    /// <summary>
    /// Saves the place for a specific user in an top list of warnings
    /// </summary>
    /// <param name="Place">The place of the user in the top list</param>
    /// <param name="UserId">The UserId of the user</param>
    /// <param name="WarningNumber">The amount of warnings that the user got</param>
    public record struct ReturnValue(uint Place, ulong UserId, uint WarningNumber);

    public async Task<IEnumerable<ReturnValue>> Execute(ITaskContext ctx, Args arg)
    {
        var (maxRankings, guildId) = arg;
        var topWarned = await ctx.Model.UserGuildStates.Where(
                state => state.GuildId == guildId & state.WarningNumber != 0)
            .OrderByDescending(state => state.WarningNumber)
            .Take(maxRankings)
            .Select(state => new ReturnValue(0, state.UserId, state.WarningNumber))
            .ToListAsync();

        return topWarned.Select((t, index) =>
        {
            t.Place = (uint) (index + 1);
            return t;
        });

    }
}