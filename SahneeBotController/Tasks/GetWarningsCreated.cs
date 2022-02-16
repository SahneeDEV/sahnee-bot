using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

public class GetWarningsCreated
{
    /// <summary>
    /// Arguments for getting all warnings created till specified dateTime
    /// </summary>
    /// <param name="Start">The dateTime object specifying which commands should be pulled</param>
    /// <param name="End">The timespan which the warnings have been created</param>
    /// <param name="GuildID">The ID of the guild in which the warnings have bin issued</param>
    public record struct Args(DateTime Start, DateTime End, ulong GuildID);

    public async Task<List<IWarning>> Execute(ITaskContext ctx, Args arg)
    {
        var (start,end, guildId) = arg;

        return await ctx.Model.Warnings.Where(
            warning => warning.GuildId == guildId &&
                       warning.Time >= start &&
                       warning.Time <= end
        ).ToListAsync<IWarning>();
    }
}