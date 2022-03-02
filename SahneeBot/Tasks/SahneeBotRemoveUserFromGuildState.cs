using Discord.WebSocket;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotRemoveUserFromGuildState : RemoveUserFromUserGuildStatesTask
{
    private readonly RemovedUsersFromGuildStateDiscordFormatter _removedUsersFromGuildStateDiscordFormatter;

    public SahneeBotRemoveUserFromGuildState(
        RemovedUsersFromGuildStateDiscordFormatter removedUsersFromGuildStateDiscordFormatter)
    {
        _removedUsersFromGuildStateDiscordFormatter = removedUsersFromGuildStateDiscordFormatter;
    }
    
    public new record struct Args(SocketMessageComponent SocketMessageComponent);
    
    public async Task Execute(ITaskContext ctx, Args arg)
    {
        var allEntries = new List<string>();
        //loop through every user
        if (arg.SocketMessageComponent.Data.Values.Count == 0)
        {
            allEntries.Add(arg.SocketMessageComponent.Data.Value);
        }
        else
        {
            allEntries.AddRange(arg.SocketMessageComponent.Data.Values);
        }

        var failCounter = 0;
        var amountRemoved = 0;
        var channel = (SocketTextChannel)arg.SocketMessageComponent.Channel;
        foreach (var currentEntry in allEntries)
        {
            if (!await Execute(ctx, new RemoveUserFromUserGuildStatesTask.Args(channel.Guild.Id
                    , ulong.Parse(currentEntry))))
            {
                failCounter++;
            }
            amountRemoved++;
        }
        await _removedUsersFromGuildStateDiscordFormatter.FormatAndSend(
            new RemovedUsersFromGuildStateDiscordFormatter.Args(channel.Guild.Id, amountRemoved, failCounter)
            , arg.SocketMessageComponent.ModifyOriginalResponseAsync);
    }
}
