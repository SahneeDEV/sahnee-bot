using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// This task gets the state of a specific guild or creates it if it does not exist.
/// </summary>
public class GetGuildStateTask: ITask<GetGuildStateTask.Args, IGuildState>
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Arguments for getting the guild state.
    /// </summary>
    public struct Args
    {
        /// <summary>
        /// The guild ID of the user.
        /// </summary>
        public readonly ulong GuildId;

        public Args(ulong guildId)
        {
            GuildId = guildId;
        }
    }

    public GetGuildStateTask(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IGuildState> Execute(ITaskContext ctx, Args args)
    {
        var guildState = await ctx.Model.GuildStates
            .FirstOrDefaultAsync(g => g.GuildId == args.GuildId);
        if (guildState != null)
        {
            //check for the warning role prefix
            if (string.IsNullOrWhiteSpace(guildState.WarningRolePrefix))
            {
                guildState.WarningRolePrefix = _configuration["BotSettings:WarningRolePrefix"];
                await ctx.Model.SaveChangesAsync();
            }
            return guildState;
        }
        guildState = new GuildState
        {
            GuildId = args.GuildId,
            WarningRolePrefix = _configuration["BotSettings:WarningRolePrefix"]
        };
        ctx.Model.GuildStates.Add(guildState);
        await ctx.Model.SaveChangesAsync();
        return guildState;
    }
}
