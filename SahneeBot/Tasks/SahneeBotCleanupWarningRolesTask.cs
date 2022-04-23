using System.Net;
using Discord;
using Discord.Net;
using Microsoft.Extensions.Logging;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

/// <summary>
/// Cleans up all unused warnings of a guild.
/// </summary>
public class SahneeBotCleanupWarningRolesTask : ITask<SahneeBotCleanupWarningRolesTask.Args, ISuccess<uint>>
{
    /// <summary>
    /// Arguments for this task.
    /// </summary>
    /// <param name="GuildId">The guild to clean in.</param>
    public record struct Args(ulong GuildId);

    private readonly Bot _bot;
    private readonly GetGuildStateTask _guildStateTask;
    private readonly ILogger<SahneeBotCleanupWarningRolesTask> _logger;
    private readonly SahneeBotDiscordError _discordError;

    public SahneeBotCleanupWarningRolesTask(Bot bot
        , GetGuildStateTask guildStateTask
        , ILogger<SahneeBotCleanupWarningRolesTask> logger
        , SahneeBotDiscordError discordError)
    {
        _bot = bot;
        _guildStateTask = guildStateTask;
        _logger = logger;
        _discordError = discordError;
    }

    public async Task<ISuccess<uint>> Execute(ITaskContext ctx, Args arg)
    {
        var guildId = arg.GuildId;
        var guild = await _bot.Client.GetGuildAsync(guildId);
        if (guild == null)
        {
            return new Error<uint>("The server could not be found.");
        }
        // Check if the guild sets new warnings as a role
        var guildState = await _guildStateTask.Execute(ctx, new GetGuildStateTask.Args(guild.Id));
        if (!guildState.SetRoles)
        {
            return new Error<uint>("The server does not use the role system.");
        }
        
        _logger.LogDebug(EventIds.Task, "Starting warning role cleanup for guild {Guild}", guild.Id);

        var prefix = guildState.WarningRolePrefix;
        var assignedRoles = new HashSet<ulong>();
        // I don't like it, but for now I don't know a better/ more efficient way
        // TODO: The API has an endpoint to easily count the number of users in roles, but Discord.NET does not support it?
        // https://discord.com/api/v8/guilds/<guildID>/roles/member-counts
        foreach (var currentUser in await guild.GetUsersAsync())
        {
            // A user can only have one warning role, otherwise something went wrong
            var userWarningRole = currentUser.RoleIds
                .Select(guild.GetRole)
                .FirstOrDefault(r => r.Name.StartsWith(prefix));
            // Check if already in the list of all roles
            if (userWarningRole == null)
            {
                continue;
            }
            assignedRoles.Add(userWarningRole.Id);
        }
            
        // Get all available warning roles in the current guild
        var availableWarningRoles = guild.Roles
            .Where(r => r.Name.StartsWith(prefix));
        // Check if every available role is assigned to a user
        var notNeededRoles = availableWarningRoles
            .Where(r => !assignedRoles.Contains(r.Id))
            .ToList();
        // Delete not needed roles
        uint deleted = 0;
        ISuccess<uint>? error = null;
        foreach (var notNeededRole in notNeededRoles)
        {
            _logger.LogDebug(EventIds.Task
                , "Attempting to delete no longer needed warning role {Role} on guild {Guild}"
                , notNeededRole.Id, guild.Id);
            try
            {
                await notNeededRole.DeleteAsync();
                deleted++;
            }
            catch (Exception exception)
            {
                error = await _discordError.TryGetError<uint>(ctx, new SahneeBotDiscordError.ErrorOptions
                {
                    Exception = exception,
                    GuildId = guildId
                });
                if (error == null)
                {
                    throw;
                }
            }
        }
        if (error != null)
        {
            return error;
        }
        if (deleted > 0)
        {
            _logger.LogDebug(EventIds.Task
                , "In Guild: {Guild} deleted roles: {DeletedRoles} - Had error? {Error}"
                , guild.Id, string.Join(",", notNeededRoles.Select(r => r.Name)), error?.Message);
        }
        return error ?? new Success<uint>(deleted);
    }
}