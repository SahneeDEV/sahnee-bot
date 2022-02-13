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
    public struct Args
    {
        /// <summary>
        /// The guild ID of the user.
        /// </summary>
        public readonly ulong GuildId;
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public readonly ulong UserId;

        public Args(ulong guildId, ulong userId)
        {
            GuildId = guildId;
            UserId = userId;
        }
    }
    
    public async Task<IUserGuildState> Execute(ITaskContext ctx, Args args)
    {
        var userGuildState = await ctx.Model.UserGuildStates
            .FirstOrDefaultAsync(s => s.GuildId == args.GuildId && s.UserId == args.UserId);
        if (userGuildState != null)
        {
            return userGuildState;
        }
        userGuildState = new UserGuildState
        {
            GuildId = args.GuildId,
            UserId = args.UserId
        };
        ctx.Model.UserGuildStates.Add(userGuildState);
        await ctx.Model.SaveChangesAsync();
        return userGuildState;
    }
}
