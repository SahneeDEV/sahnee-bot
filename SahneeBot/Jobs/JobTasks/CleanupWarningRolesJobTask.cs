using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBotModel;
using IRole = Discord.IRole;

namespace SahneeBot.Jobs.JobTasks;

public class CleanupWarningRolesJobTask
{
    private readonly ILogger<CleanupWarningRolesJobTask> _logger;
    private readonly DiscordSocketClient _bot;
    private readonly string _warningPrefix;

    public CleanupWarningRolesJobTask(ILogger<CleanupWarningRolesJobTask> logger, DiscordSocketClient bot
        , IConfiguration configuration)
    {
        _logger = logger;
        _bot = bot;

        //set the warning prefix
        _warningPrefix = configuration["BotSettings:WarningRolePrefix"];
    }

    /// <summary>
    /// Cleans all not needed warning roles on a server
    /// </summary>
    public async Task CleanupWarningRolesRun(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SahneeBotModelContext>();
                
            _logger.LogDebug(EventIds.Jobs, "Starting Role deletion on Guilds");
            foreach (var currentGuild in _bot.Guilds)
            {
                await CleanupWarningRolesForAGuild(currentGuild, context);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(EventIds.Jobs, e, "Failed Job CleanupWarningRolesRun!");
        }
    }

    /// <summary>
    /// Cleans all warning roles for a single guild
    /// </summary>
    /// <param name="currentGuild"></param>
    /// <param name="context"></param>
    public async Task<int> CleanupWarningRolesForAGuild(SocketGuild? currentGuild, SahneeBotModelContext context)
    {
        try
        {
            if (currentGuild == null)
            {
                return 0;
            }
            //check if the guild sets new warnings as a role
            var guildState = await context.GuildStates.FirstAsync(g => g.GuildId == currentGuild.Id);
            if (!guildState.SetRoles)
            {
                return 0;
            }
            //get all roles that are assigned to guildUsers
            await currentGuild.DownloadUsersAsync();
            var assignedRoles = new List<IRole>();
            //i don't like it, but for now I don't know a better/ more efficient way
            foreach (var currentUser in currentGuild.Users)
            {
                //a user can only have one warning role, otherwise something went wrong
                var userWarningRole = currentUser.Roles
                    .FirstOrDefault(r => r.Name.StartsWith(_warningPrefix));
                //check if already in the list of all roles
                if (userWarningRole == null)
                {
                    continue;
                }
                if (!assignedRoles.Contains(userWarningRole))
                {
                    assignedRoles.Add(userWarningRole);
                }
            }
            //get all available warning roles in the current guild
            var availableWarningRoles = currentGuild.Roles
                .Where(r => r.Name
                    .StartsWith(_warningPrefix));

            //check if every available role is assigned to a user
            var notNeededRoles = availableWarningRoles
                .Where(r => !assignedRoles.Contains(r)).ToList();
                    
            //delete not needed roles
            await Task.WhenAll(notNeededRoles.Select(r => r.DeleteAsync()).ToArray());
            if (notNeededRoles.Count > 0)
            {
                _logger.LogDebug(EventIds.Jobs, "In Guild: {guild} deleted roles: {deletedRoles}"
                    , currentGuild.Id, string.Join(",", notNeededRoles.Select(r => r.Name)));
            }
            return notNeededRoles.Count;
        }
        catch (Exception e)
        {
            _logger.LogError(EventIds.Jobs, e, "Failed cleaning warning roles in guild {guildId}"
                , currentGuild!.Id);
            return 0;
        }
    }
}
