using Discord;
using Discord.WebSocket;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotGetLeftGuildUsers
{
    private readonly GetGuildGuildUsersTask _getGuildGuildUsers;
    private readonly DiscordSocketClient _bot;

    public record struct Args(ulong GuildId);
    
    public SahneeBotGetLeftGuildUsers(GetGuildGuildUsersTask getGuildGuildUsers
        , DiscordSocketClient bot)
    {
        _getGuildGuildUsers = getGuildGuildUsers;
        _bot = bot;
    }

    public async Task<List<IUser>> Execute(ITaskContext ctx, Args args)
    {
        var userIds = await _getGuildGuildUsers.Execute(ctx, new GetGuildGuildUsersTask.Args(args.GuildId));
        var users = new List<IUser>();
        foreach (var currentId in userIds)
        {
            users.Add(await _bot.GetUserAsync(currentId));
        }
        return users;
    }
}
