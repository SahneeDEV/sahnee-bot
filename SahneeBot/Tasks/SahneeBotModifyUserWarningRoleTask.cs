using ColorHelper;
using Discord;
using Discord.Net;
using System.Net;
using Microsoft.Extensions.Logging;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotModifyUserWarningRoleTask : ModifyUserWarningRoleTask
{
    private readonly GetGuildStateTask _guildState;
    private readonly Bot _bot;
    private readonly ILogger<SahneeBotModifyUserWarningRoleTask> _logger;
    private readonly CheckRoleLimitTask _checkRoleLimitTask;
    private readonly SahneeBotDiscordError _discordError;

    public SahneeBotModifyUserWarningRoleTask(GetGuildStateTask guildStateTask
        , Bot bot
        , ILogger<SahneeBotModifyUserWarningRoleTask> logger
        , CheckRoleLimitTask checkRoleLimitTask
        , SahneeBotDiscordError discordError)
    {
        _guildState = guildStateTask;
        _bot = bot;
        _logger = logger;
        _checkRoleLimitTask = checkRoleLimitTask;
        _discordError = discordError;
    }

    public override async Task<ISuccess<ulong>> Execute(ITaskContext ctx, Args args)
    {
        // Don't set a role if the guild opted out
        var guildState = await _guildState.Execute(ctx, new GetGuildStateTask.Args(args.GuildId));
        if (!guildState.SetRoles)
        {
            return new Success<ulong>(0);
        }
        
        // Check if the guild exists
        var currentGuild = await _bot.Client.GetGuildAsync(args.GuildId);
        if (currentGuild == null)
        {
            return new Error<ulong>("Could not find server.");
        }
        
        // Does the user exist in the guild?
        var currentGuildUser = await currentGuild.GetUserAsync(args.UserId);
        if (currentGuildUser == null)
        {
            return new Error<ulong>("Could not find user.");
        }
        
        // Remove all current warning roles from the user if any are available
        var newRoleName = guildState.WarningRolePrefix + args.Number;
        var rolesToRemove = currentGuildUser.RoleIds
            .Select(role => currentGuild.GetRole(role))
            .Where(role => role != null && role.Name.StartsWith(guildState.WarningRolePrefix))
            .ToList();
        var removeRolesCommand = Command.CreateSimple(
            async () =>
            {
                _logger.LogDebug(EventIds.Task
                    , "Removing roles of user {User} in {Guild}"
                    , currentGuildUser.Id, args.GuildId);
                await Task.WhenAll(rolesToRemove.Select(role => currentGuildUser.RemoveRoleAsync(role)));
            },
            async () =>
            {
                _logger.LogDebug(EventIds.Task
                    , "Undo removing roles of user {User} in {Guild}"
                    , currentGuildUser.Id, args.GuildId);
                await Task.WhenAll(rolesToRemove.Select(role => currentGuildUser.AddRoleAsync(role)));
            });
        try
        {
            await removeRolesCommand.Do();
        }
        catch (Exception exception)
        {
            _logger.LogError(EventIds.Command
                , exception, "Failed to remove a role from a user in guild {Guild}"
                , args.GuildId);
            try
            {
                await removeRolesCommand.Undo();
            }
            catch (Exception)
            {
                // ignored
            }
            var error = await _discordError.TryGetError<ulong>(ctx, new SahneeBotDiscordError.ErrorOptions
            {
                Exception = exception,
                GuildId = args.GuildId
            });
            if (error != null)
            {
                return error;
            }
            throw;
        }
        
        // Check if the new amount is 0
        if (args.Number == 0)
        {
            return new Success<ulong>(0);
        }
        
        // Check if the new warning as group already exists
        IRole? newRole;
        if (currentGuild.Roles.All(r => r.Name != newRoleName))
        {
            try
            {
                // Get custom color if available
                var roleColor = Color.LightGrey;
                if (!string.IsNullOrWhiteSpace(guildState.WarningRoleColor))
                {
                    var rgb = ColorConverter.HexToRgb(new HEX(guildState.WarningRoleColor));
                    roleColor = new Color(rgb.R, rgb.G, rgb.B);
                }
                // Create the new role
                newRole = await currentGuild
                    .CreateRoleAsync(newRoleName, default,
                        roleColor, false, null);
                // Add the new role to the guild
                await currentGuildUser.AddRoleAsync(newRole);
                await _checkRoleLimitTask.Execute(ctx, new CheckRoleLimitTask.Args(currentGuild.Id));
                return new Success<ulong>(newRole.Id);
            }
            catch (Exception exception)
            {
                _logger.LogError(EventIds.Command
                    , exception, "Failed to create and add a new role to a guid {Guild}"
                    , args.GuildId);
                await removeRolesCommand.Undo();
                var error = await _discordError.TryGetError<ulong>(ctx, new SahneeBotDiscordError.ErrorOptions
                {
                    Exception = exception,
                    GuildId = args.GuildId
                });
                if (error != null)
                {
                    return error;
                }
                throw;
            }
        }
        // Check if the role got created or needs to be received from the guild
        newRole = currentGuild.Roles.First(r => r.Name == newRoleName);
        // Add the role to the user
        try
        {
            await currentGuildUser.AddRoleAsync(newRole);
            return new Success<ulong>(newRole.Id);
        }
        catch (Exception exception)
        {
            _logger.LogError(EventIds.Command
                , exception, "Failed to add a role to a user in guild {Guild}"
                , args.GuildId);
            await removeRolesCommand.Undo();
            var error = await _discordError.TryGetError<ulong>(ctx, new SahneeBotDiscordError.ErrorOptions
            {
                Exception = exception,
                GuildId = args.GuildId
            });
            if (error != null)
            {
                return error;
            }
            throw;
        }
    }
}

