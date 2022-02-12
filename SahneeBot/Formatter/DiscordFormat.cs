using Discord;
using Discord.Interactions;

namespace SahneeBot.Formatter;

/// <summary>
/// A discord message format.
/// </summary>
public class DiscordFormat
{
    /// <summary>
    /// The text of the message.
    /// </summary>
    public string? Text = null;
    /// <summary>
    /// The embeds of the message.
    /// </summary>
    public Embed[]? Embeds = null;
    /// <summary>
    /// Settings for allowed mentions.
    /// </summary>
    public AllowedMentions? AllowedMentions = null;
    /// <summary>
    /// The options of the request.
    /// </summary>
    public RequestOptions? Options = null;
    /// <summary>
    /// The message components.
    /// </summary>
    public MessageComponent? Components = null;
    /// <summary>
    /// A single embed of the message. Alias for creating an Embed array with a single element.
    /// </summary>
    public Embed? Embed = null;

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
        await del(Text, Embeds, sendOptions.IsTts, sendOptions.Ephemeral, AllowedMentions, Options, Components, Embed);
    }
    public async Task Send(SendMessageAsyncDelegate del, SendOptions sendOptions = default)
    {
        await del(Text, sendOptions.IsTts, Embed, Options, AllowedMentions, Components, Embeds);
    }
}
