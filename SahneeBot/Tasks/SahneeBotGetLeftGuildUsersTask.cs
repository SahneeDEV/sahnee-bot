﻿using Discord;
using Discord.WebSocket;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

/// <summary>
/// Removes all selected users from the guild state and removes the warnings from the user from the guild
/// </summary>
public class SahneeBotGetLeftGuildUsersTask : ITask<SahneeBotGetLeftGuildUsersTask.Args, IEnumerable<IUser>>
{
    private readonly GetGuildGuildUsersTask _getGuildGuildUsers;
    private readonly Bot _bot;

    public record struct Args(ulong GuildId);
    
    public SahneeBotGetLeftGuildUsersTask(GetGuildGuildUsersTask getGuildGuildUsers
        , Bot bot)
    {
        _getGuildGuildUsers = getGuildGuildUsers;
        _bot = bot;
    }

    public async Task<IEnumerable<IUser>> Execute(ITaskContext ctx, Args args)
    {
        var userIds = await _getGuildGuildUsers.Execute(ctx, new GetGuildGuildUsersTask.Args(args.GuildId));
        return await Task.WhenAll(userIds.Select(id => _bot.Client.GetUserAsync(id)));
    }
}
