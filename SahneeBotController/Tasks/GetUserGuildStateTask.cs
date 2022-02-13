using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// This task gets the amount of warnings of the given user.
/// </summary>
public class GetUserGuildStateTask: ITask<GetUserGuildStateTask.Args, IUserGuildState>
{
    public struct Args
    {
        public readonly ulong GuildId;
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
        return userGuildState;
    }
}