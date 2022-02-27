using Discord;

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
    /// <param name="sendOptions">Options to send the message</param>
    /// <typeparam name="T">The data type to format.</typeparam>
    public static async Task FormatAndSend<T>(this IDiscordFormatter<T> discordFormatter, T arg, 
        DiscordFormat.RespondAsyncDelegate del, DiscordFormat.SendOptions sendOptions = default)
    {
        var format = await discordFormatter.Format(arg);
        await format.Send(del, sendOptions);
    }
    public static async Task FormatAndSend<T>(this IDiscordFormatter<T> discordFormatter, T arg, 
        DiscordFormat.SendMessageAsyncDelegate del, DiscordFormat.SendOptions sendOptions = default)
    {
        var format = await discordFormatter.Format(arg);
        await format.Send(del, sendOptions);
    }
    public static async Task FormatAndSend<T>(this IDiscordFormatter<T> discordFormatter, T arg, 
        DiscordFormat.SendChannelMessageAsyncDelegate del, DiscordFormat.SendOptions sendOptions = default)
    {
        var format = await discordFormatter.Format(arg);
        await format.Send(del, sendOptions);
    }
    public static async Task FormatAndSend<T>(this IDiscordFormatter<T> discordFormatter, T arg, 
        DiscordFormat.SendWebhookMessageAsyncDelegate del, DiscordFormat.SendOptions sendOptions = default)
    {
        var format = await discordFormatter.Format(arg);
        await format.Send(del, sendOptions);
    }
    public static async Task FormatAndSend<T>(this IDiscordFormatter<T> discordFormatter, T arg, 
        DiscordFormat.ModifyOriginalResponseAsyncDelegate del, DiscordFormat.SendOptions sendOptions = default)
    {
        var format = await discordFormatter.Format(arg);
        await format.Send(del, sendOptions);
    }
    public static async Task FormatAndSend<T>(this IDiscordFormatter<T> discordFormatter, T arg, 
        DiscordFormat.CustomDelegate del, DiscordFormat.SendOptions sendOptions = default)
    {
        var format = await discordFormatter.Format(arg);
        await format.Send(del, sendOptions);
    }

    public static async Task<IEnumerable<DiscordFormat>> FormatMany<T>(this IDiscordFormatter<T> discordFormatter,
        IEnumerable<T> args)
    {
        var formats = await Task.WhenAll(args.Select(discordFormatter.Format));
        return DiscordFormat.Join(formats);
    } 

    /// <summary>
    /// Formats and sends many messages. Use this method when wanting to join multiple messages together.
    /// </summary>
    /// <param name="discordFormatter">The discord formatter to use.</param>
    /// <param name="args">The arguments to format and send.</param>
    /// <param name="delFirst">The delegate to send the first message.</param>
    /// <param name="delOther">The delegate to send all other messages.</param>
    /// <param name="sendOptions">Options to send the message</param>
    /// <returns>Were any messages sent?</returns>
    /// <typeparam name="T">The data type.</typeparam>
    public static async Task<bool> FormatAndSendMany<T>(this IDiscordFormatter<T> discordFormatter, IEnumerable<T> args,
        DiscordFormat.ModifyOriginalResponseAsyncDelegate delFirst,
        DiscordFormat.SendChannelMessageAsyncDelegate delOther, DiscordFormat.SendOptions sendOptions = default)
    {
        var formats = await discordFormatter.FormatMany(args);
        var sentAny = false;
        foreach (var format in formats)
        {
            if (!sentAny)
            {
                await format.Send(delFirst, sendOptions);
                sentAny = true;
            }
            else
            {
                await format.Send(delOther, sendOptions);
            }
        }

        return sentAny;
    }

    /// <summary>
    /// Formats and sends many messages. Use this method when wanting to join multiple messages together.
    /// </summary>
    /// <param name="discordFormatter">The discord formatter to use.</param>
    /// <param name="args">The arguments to format and send.</param>
    /// <param name="del">The delegate to send the messages.</param>
    /// <param name="sendOptions">Options to send the message</param>
    /// <param name="beforeSendFirst">Called before sending the first entry (if any)</param>
    /// <returns>Were any messages sent?</returns>
    /// <typeparam name="T">The data type.</typeparam>
    public static async Task<bool> FormatAndSendMany<T>(
        this IDiscordFormatter<T> discordFormatter,
        IEnumerable<T> args,
        DiscordFormat.SendChannelMessageAsyncDelegate del,
        DiscordFormat.SendOptions sendOptions = default,
        Func<Task>? beforeSendFirst = null
        )
    {
        var formats = await discordFormatter.FormatMany(args);
        var sentAny = false;
        foreach (var format in formats)
        {
            if (!sentAny && beforeSendFirst != null)
            {
                await beforeSendFirst();
            }
            await format.Send(del, sendOptions);
            sentAny = true;
        }

        return sentAny;
    }
}