using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

/// <summary>
/// Sends the given message to the selected members in a guild. Returns the amount of users that have received a message
/// if successful. Is also successful if zero users were selected or no user could receive the message (e.g. due to
/// blocking the bot or opting out of messages)
/// </summary>
public class SahneeBotPrivateMessageToGuildMembersTask : ITask<SahneeBotPrivateMessageToGuildMembersTask.Args, ISuccess<uint>>
{
    private readonly DiscordSocketClient _bot;
    private readonly ILogger<SahneeBotPrivateMessageToGuildMembersTask> _logger;
    private readonly GetMessageOptOutTask _messageOptOutTask;
    private readonly SendMessageOptOutHintToUserTask _sendMessageOptOutHintToUserTask;

    public delegate Task<IEnumerable<IUser>> GetUsersDelegate(IGuild guild);

    public record struct Args(ulong GuildId
        , GetUsersDelegate GetUsers
        , IEnumerable<DiscordFormat> Message
        , bool IgnoreBot = false
        , bool IgnoreOptOut = false);
    
    public SahneeBotPrivateMessageToGuildMembersTask(DiscordSocketClient bot
        , ILogger<SahneeBotPrivateMessageToGuildMembersTask> logger
        , GetMessageOptOutTask messageOptOutTask
        , SendMessageOptOutHintToUserTask sendMessageOptOutHintToUserTask)
    {
        _bot = bot;
        _logger = logger;
        _messageOptOutTask = messageOptOutTask;
        _sendMessageOptOutHintToUserTask = sendMessageOptOutHintToUserTask;
    }


    public async Task<ISuccess<uint>> Execute(ITaskContext ctx, Args arg)
    {
        var guild = _bot.GetGuild(arg.GuildId);
        if (guild == null)
        {
            return new Error<uint>("Could not find Server");
        }

        var users = await arg.GetUsers(guild);
        uint ok = 0;
        foreach (var user in users)
        {
            if (user.IsBot && !arg.IgnoreBot)
            {
                continue;
            }
            
            var optedOut = await _messageOptOutTask.Execute(ctx, 
                new GetMessageOptOutTask.Args(user.Id, guild.Id));
            if (optedOut && !arg.IgnoreOptOut)
            {
                continue;
            }

            var sent = false;
            try
            {
                _logger.LogDebug(EventIds.Task
                    , "Sending private message to user {User} in guild {Guild}"
                    , user.Id, guild.Id);
                sent = await arg.Message.SendMany(user.SendMessageAsync);
            }
            catch (Exception e)
            {
                _logger.LogWarning(EventIds.Task, e
                    , "Failed to send private message to user {User} in guild {Guild}"
                    , user.Id, guild.Id);
            }

            if (sent)
            {
                ok++;
                await _sendMessageOptOutHintToUserTask.Execute(ctx, new SendMessageOptOutHintToUserTask.Args(
                    user.Id, guild.Id));
            }
        }

        return new Success<uint>(ok);
    }
}