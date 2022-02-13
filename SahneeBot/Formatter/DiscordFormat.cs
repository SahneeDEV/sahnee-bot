using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// A discord message format.
/// </summary>
public class DiscordFormat
{
    /// <summary>
    /// The text of the message.
    /// </summary>
    public string? Text;
    /// <summary>
    /// The embeds of the message.
    /// </summary>
    public Embed[]? Embeds = null;
    /// <summary>
    /// Settings for allowed mentions.
    /// </summary>
    public AllowedMentions? AllowedMentions = null;
    /// <summary>
    /// The message components.
    /// </summary>
    public MessageComponent? Components = null;
    /// <summary>
    /// A single embed of the message. Alias for creating an Embed array with a single element.
    /// </summary>
    public Embed? Embed;

    /// <summary>
    /// Creates an empty format.
    /// </summary>
    public DiscordFormat()
    {
    }

    /// <summary>
    /// Creates a format with the given text.
    /// </summary>
    /// <param name="text">The text.</param>
    public DiscordFormat(string text)
    {
        Text = text;
    }

    /// <summary>
    /// Creates a format with the given embed.
    /// </summary>
    /// <param name="embed">The embed.</param>
    public DiscordFormat(Embed embed)
    {
        Embed = embed;
    }

    /// <summary>
    /// Options for sending the message.
    /// </summary>
    public struct SendOptions
    {
        /// <summary>
        /// Can only the command sender see the response?
        /// </summary>
        public bool Ephemeral = false;
        /// <summary>
        /// Ready the message?
        /// </summary>
        public bool IsTts = false;
        /// <summary>
        /// The options of the request.
        /// </summary>
        public RequestOptions? Request = null;
    }

    public delegate Task RespondAsyncDelegate(
        string? text = null,
        Embed[]? embeds = null,
        bool isTts = false,
        bool ephemeral = false,
        AllowedMentions? allowedMentions = null,
        RequestOptions? options = null,
        MessageComponent? components = null,
        Embed? embed = null);

    public delegate Task SendMessageAsyncDelegate(
        string? text = null,
        bool isTts = false,
        Embed? embed = null,
        RequestOptions? options = null,
        AllowedMentions? allowedMentions = null,
        MessageComponent? components = null,
        Embed[]? embeds = null);

    public async Task Send(RespondAsyncDelegate del, SendOptions sendOptions = default)
    {
        await del(Text, Embeds, sendOptions.IsTts, sendOptions.Ephemeral, AllowedMentions, sendOptions.Request, 
            Components, Embed);
    }
    public async Task Send(SendMessageAsyncDelegate del, SendOptions sendOptions = default)
    {
        await del(Text, sendOptions.IsTts, Embed, sendOptions.Request, AllowedMentions, Components, Embeds);
    }
}
