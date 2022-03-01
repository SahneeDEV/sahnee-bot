using Microsoft.EntityFrameworkCore;

namespace SahneeBotController.Tasks;

public class GetGuildGuildUsersTask : ITask<GetGuildGuildUsersTask.Args, List<ulong>>
{
    /// <summary>
    /// Arguments to get all users currently having warnings to the guild.
    /// </summary>
    /// <param name="GuildId">the guild to get the users from</param>
    public record struct Args(ulong GuildId);

    
    public async Task<List<ulong>> Execute(ITaskContext ctx, Args args)
    {
        var usersWithWarningsInGuild = 
            await ctx.Model.UserGuildStates
                .Where(g => g.GuildId == args.GuildId)
                .ToListAsync();

        var userIds = new List<ulong>();
        foreach (var currentGuildState in usersWithWarningsInGuild)
        {
            userIds.Add(currentGuildState.UserId);
        }

        return userIds;
    }
}
