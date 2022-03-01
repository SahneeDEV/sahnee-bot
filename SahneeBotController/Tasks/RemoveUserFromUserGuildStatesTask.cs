using Microsoft.EntityFrameworkCore;

namespace SahneeBotController.Tasks;

/// <summary>
/// Removes a user and all his associated warnings from a guild
/// </summary>
public class RemoveUserFromUserGuildStatesTask : ITask<RemoveUserFromUserGuildStatesTask.Args, bool>
{

    /// <summary>
    /// Arguments for getting the user removed
    /// </summary>
    /// <param name="GuildId"></param>
    /// <param name="UserId"></param>
    public record struct Args(ulong GuildId, ulong UserId);

    public async Task<bool> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, userId) = arg;
        var entryToRemove = await ctx.Model.UserGuildStates
            .FirstOrDefaultAsync(ug => ug.GuildId == guildId 
                         && ug.UserId == userId);
        var warningsToRemove = await ctx.Model.Warnings.Where(w => 
            w.UserId == arg.UserId && w.GuildId == arg.GuildId)
            .ToListAsync();

        if (entryToRemove != null)
        {
            ctx.Model.UserGuildStates.Remove(entryToRemove);
        }
        if (warningsToRemove.Count > 0)
        {
            ctx.Model.Warnings.RemoveRange(warningsToRemove);
        }
        await ctx.Model.SaveChangesAsync();
        return true;

    }
}
