using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// This task gets the state of a user on a single guild or creates it if it does not exist.
/// </summary>
public class GetUserGuildStateTask: ITask<GetUserGuildStateTask.Args, IUserGuildState>
{
    /// <summary>
    /// Arguments for getting the user guild state.
    /// </summary>
    public record struct Args(ulong GuildId, ulong UserId);
    
    public async Task<IUserGuildState> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, userId) = arg;
        var userGuildState = await ctx.Model.UserGuildStates
            .FirstOrDefaultAsync(s => s.GuildId == guildId && s.UserId == userId);
        if (userGuildState != null)
        {
            return userGuildState;
        }
        userGuildState = new UserGuildState
        {
            GuildId = guildId,
            UserId = userId
        };
        ctx.Model.UserGuildStates.Add(userGuildState);
        await ctx.Model.SaveChangesAsync();
        return userGuildState;
    }
}
