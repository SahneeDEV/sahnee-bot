using ColorHelper;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotModifyWarningGroupTask : ModifyUserWarningGroupTask
{
    private readonly GetGuildStateTask _guildState;
    private readonly Bot _bot;
    private readonly ILogger<SahneeBotModifyWarningGroupTask> _logger;

    public SahneeBotModifyWarningGroupTask(GetGuildStateTask guildStateTask
        , Bot bot
        , ILogger<SahneeBotModifyWarningGroupTask> logger)
    {
        _guildState = guildStateTask;
        _bot = bot;
        _logger = logger;
    }

    public override async Task<ISuccess<ulong>> Execute(ITaskContext ctx, Args args)
    {
        // Don't set a role if the guild opted out
        var guildState = await _guildState.Execute(ctx,
            new GetGuildStateTask.Args(args.GuildId));
        if (!guildState.SetRoles)
        {
            return new Error<ulong>("Role handling has been disabled for this server.");
        }
        // Check if the new role already exists on the guild
        var currentGuild = await _bot.Client.GetGuildAsync(args.GuildId);
        if (currentGuild == null)
        {
            return new Error<ulong>("Could not find server.");
        }
        var currentGuildUser = await currentGuild.GetUserAsync(args.UserId);
        if (currentGuildUser == null)
        {
            return new Error<ulong>("Could not find user.");
        }
        var newRoleName = guildState.WarningRolePrefix + args.Number;
        // Remove all current warning roles from the user if any are available
        foreach (var currentRoleId in currentGuildUser.RoleIds)
        {
            var currentRole = currentGuild.GetRole(currentRoleId);
            if (currentRole.Name.StartsWith(guildState.WarningRolePrefix))
            {
                // Remove the role
                await currentGuildUser.RemoveRoleAsync(currentRole);
            }
        }
        // Check if the new amount is 0
        if (args.Number == 0)
        {
            return new Error<ulong>("The user does not have any warnings.");
        }
        // Check if the new warning as group already exists
        IRole? newRole;
        if (currentGuild.Roles.Any(r => r.Name != newRoleName))
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
                //create the new role
                newRole = await currentGuild
                    .CreateRoleAsync(newRoleName, default,
                        roleColor, false, null);
                //add the new role to the guild
                await currentGuildUser.AddRoleAsync(newRole);
                return new Success<ulong>(newRole.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(EventIds.Command, e, 
                    "Failed to create and add a new role to a guid {Guild}", args.GuildId);
                return new Error<ulong>(e.Message);
            }
        }
        //check if the role got created or needs to be received from the guild
        newRole = currentGuild.Roles.First(r => r.Name == newRoleName);
        //add the role to the user
        try
        {
            await currentGuildUser.AddRoleAsync(newRole);
            return new Success<ulong>(newRole.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(EventIds.Command, e,
                "Failed to add a role to a user in guild {Guild}", args.GuildId);
            return new Error<ulong>(e.Message);
        }
    }
}
