using Microsoft.EntityFrameworkCore;

namespace SahneeBotController.Tasks;

/// <summary>
/// Returns a list of users with an entry for the guild
/// </summary>
public class GetGuildGuildUsersTask : ITask<GetGuildGuildUsersTask.Args, IEnumerable<ulong>>
{
    /// <summary>
    /// Arguments to get all users currently having warnings to the guild.
    /// </summary>
    /// <param name="GuildId">the guild to get the users from</param>
    public record struct Args(ulong GuildId);

    
    public async Task<IEnumerable<ulong>> Execute(ITaskContext ctx, Args args)
    {
        var usersWithWarningsInGuild = 
            await ctx.Model.UserGuildStates
                .Where(g => g.GuildId == args.GuildId)
                .ToListAsync();

        return usersWithWarningsInGuild.Select(currentGuildState => currentGuildState.UserId);
    }
}
