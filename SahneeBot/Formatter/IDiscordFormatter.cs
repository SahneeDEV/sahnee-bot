namespace SahneeBot.Formatter;

/// <summary>
/// A formatter class for data.
/// </summary>
/// <typeparam name="T">The data type to format.</typeparam>
public interface IDiscordFormatter<in T>
{
    /// <summary>
    /// Formats the given message.
    /// </summary>
    /// <param name="arg">The data to format.</param>
    /// <returns>The discord message.</returns>
    Task<DiscordFormat> Format(T arg);
}

/// <summary>
/// Extension methods related to discord formatting.
/// </summary>
public static class DiscordFormatterExtensions
{
    /// <summary>
    /// Formats and send the message.
    /// </summary>
    /// <param name="discordFormatter">The formatter.</param>
    /// <param name="arg">The formatter argument.</param>
    /// <param name="del">The delegate.</param>
    /// <typeparam name="T">The data type to format.</typeparam>
    public static async Task FormatAndSend<T>(this IDiscordFormatter<T> discordFormatter, T arg, DiscordFormat.RespondAsyncDelegate del)
    {
        var format = await discordFormatter.Format(arg);
        await format.Send(del);
    }
    public static async Task FormatAndSend<T>(this IDiscordFormatter<T> discordFormatter, T arg, DiscordFormat.SendMessageAsyncDelegate del)
    {
        var format = await discordFormatter.Format(arg);
        await format.Send(del);
    }
}