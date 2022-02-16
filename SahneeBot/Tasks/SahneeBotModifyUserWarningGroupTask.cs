﻿using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotModifyWarningGroupTask : ModifyUserWarningGroupTask
{
    private readonly GetUserGuildStateTask _userGuildState;
    private readonly GetGuildStateTask _guildState;
    private readonly DiscordSocketClient _bot;
    private readonly ILogger<SahneeBotModifyWarningGroupTask> _logger;
    private readonly IConfiguration _configuration;

    public SahneeBotModifyWarningGroupTask(
        GetUserGuildStateTask userGuildStateTask, GetGuildStateTask guildStateTask,
        DiscordSocketClient bot, ILogger<SahneeBotModifyWarningGroupTask> logger, IConfiguration configuration)
    {
        _userGuildState = userGuildStateTask;
        _guildState = guildStateTask;
        _bot = bot;
        _logger = logger;
        _configuration = configuration;
    }

    public override async Task<bool> Execute(ITaskContext ctx, Args args)
    {
        //Don't set a role if the guild opted out
        var guildState = await _guildState.Execute(ctx,
            new GetGuildStateTask.Args(args.GuildId));
        if (!guildState.SetRoles)
        {
            return false;
        }
        //check if the new role already exists on the guild
        var currentGuild = _bot.GetGuild(args.GuildId);
        var currentGuildUser = currentGuild.GetUser(args.UserId);
        string newRoleName = _configuration["BotSettings:WarningRolePrefix"] + args.Number;
        //remove all current warning roles from the user if any are available
        if (currentGuildUser.Roles.Any(r => 
                r.Name.Contains(_configuration["BotSettings:WarningRolePrefix"])))
        {
            foreach (var currentRole in currentGuildUser.Roles)
            {
                if (currentRole.Name.StartsWith(_configuration["BotSettings:WarningRolePrefix"]))
                {
                    //remove the role
                    await currentGuildUser.RemoveRoleAsync(currentRole);
                }
            }
        }
        //check if the new amount is 0
        if (args.Number == 0)
        {
            return false;
        }
        //check if the new warning as group already exists
        IRole? newRole = null;
        if (currentGuild.Roles.All(r => r.Name != newRoleName))
        {
            try
            {
                //create the new role
                newRole = await currentGuild
                    .CreateRoleAsync(newRoleName, default,
                        Color.LightGrey, false, null);
                //add the new role to the guild
                await currentGuildUser.AddRoleAsync(newRole);
            }
            catch (Exception e)
            {
                _logger.LogError(EventIds.Command, e, 
                    "Failed to create and add a new role to a guid {guild}", args.GuildId);
                return false;
            }
        }
        //check if the role got created or needs to be received from the guild
        newRole ??= currentGuild.Roles.FirstOrDefault(r => r.Name == newRoleName);
        //add the role to the user
        try
        {
            await currentGuildUser.AddRoleAsync(newRole);
        }
        catch (Exception e)
        {
            _logger.LogError(EventIds.Command, e,
                "Failed to add a role to a user in guild {guild}", args.GuildId);
            return false;
        }
        return true;
    }
}