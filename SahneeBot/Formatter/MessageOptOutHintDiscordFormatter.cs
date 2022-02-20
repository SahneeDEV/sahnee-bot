using Discord.WebSocket;

namespace SahneeBot.Formatter;

public class MessageOptOutHintDiscordFormatter : IDiscordFormatter<MessageOptOutHintDiscordFormatter.Args>
{
    private readonly DiscordSocketClient _bot;

    /// <summary>
    /// Arguments for this formatter.
    /// </summary>
    /// <param name="UserId">The user ID that can opt out and should receive the message.</param>
    /// <param name="GuildId">The guild ID the user can opt out of.</param>
    public record struct Args(ulong UserId, ulong GuildId);
    
    public MessageOptOutHintDiscordFormatter(DiscordSocketClient bot)
    {
        _bot = bot;
    }

    public async Task<DiscordFormat> Format(Args arg)
    {
        var (userId, guildId) = arg;
        var user = await _bot.GetUserAsync(userId);
        var guild = _bot.GetGuild(guildId);
        return new DiscordFormat($"Hello there, {user.Mention}! You have just received your first warning/message from *{guild.Name}*.\nIf you do not want me to send you messages for warnings you receive from *{guild.Name}* please open any text channel on this server and type `/config pm opt-out`.\nPlease keep in mind that opting out will **not** prevent you from receiving warnings, you just won't be informed about them.");
    }
}