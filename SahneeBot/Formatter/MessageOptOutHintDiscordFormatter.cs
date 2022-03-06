namespace SahneeBot.Formatter;

/// <summary>
/// Formats a hint about how to opt out of bot messages for a guild.
/// </summary>
public class MessageOptOutHintDiscordFormatter : IDiscordFormatter<MessageOptOutHintDiscordFormatter.Args>
{
    private readonly Bot _bot;
    private readonly DefaultFormatArguments _fmt;

    /// <summary>
    /// Arguments for this formatter.
    /// </summary>
    /// <param name="UserId">The user ID that can opt out and should receive the message.</param>
    /// <param name="GuildId">The guild ID the user can opt out of.</param>
    public record struct Args(ulong UserId, ulong GuildId);
    
    public MessageOptOutHintDiscordFormatter(Bot bot
        , DefaultFormatArguments fmt)
    {
        _bot = bot;
        _fmt = fmt;
    }

    public async Task<DiscordFormat> Format(Args arg)
    {
        var (userId, guildId) = arg;
        var user = await _bot.Client.GetUserAsync(userId);
        var guild = await _bot.Client.GetGuildAsync(guildId);
        return new DiscordFormat($"Hello there, {_fmt.GetMention(user)}! You have just received your first " +
                                 $"warning/message from {_fmt.GetMention(guild)}.\n" +
                                 "If you do not want me to send you messages for warnings you receive from " +
                                 $"{_fmt.GetMention(guild)} please open any text channel on this server and type " +
                                 "`/config pm opt-out`.\nPlease keep in mind that opting out will **not** prevent you" +
                                 " from receiving warnings, you just won't be informed about them.");
    }
}