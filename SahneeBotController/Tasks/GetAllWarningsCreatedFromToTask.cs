using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Returns all warnings today for a specific guild if created in a specific timeframe
/// </summary>
public class GetAllWarningsCreatedFromToTask: ITask<GetAllWarningsCreatedFromToTask.Args, IEnumerable<IWarning>>
{
    /// <summary>
    /// Arguments for getting all warnings created till specified dateTime
    /// </summary>
    /// <param name="Start">The dateTime object specifying which commands should be pulled</param>
    /// <param name="End">The timespan which the warnings have been created</param>
    /// <param name="GuildId">The ID of the guild in which the warnings have been issued</param>
    /// <param name="UserId">The ID of the user that got warned</param>
    public record struct Args(DateTime Start, DateTime End, ulong GuildId, ulong? UserId);

    public async Task<IEnumerable<IWarning>> Execute(ITaskContext ctx, Args arg)
    {
        var (start,end, guildId, userId) = arg;

        return await ctx.Model.Warnings.Where(
            warning => warning.GuildId == guildId &&
                       (!userId.HasValue || warning.UserId == userId.Value) &&
                       warning.Time >= start &&
                       warning.Time <= end
        ).ToListAsync<IWarning>();
    }
}