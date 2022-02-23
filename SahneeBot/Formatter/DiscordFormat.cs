using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// A discord message format.
/// </summary>
public class DiscordFormat
{
    /// <summary>
    /// Voids the message.
    /// </summary>
    public static readonly CustomDelegate Void = (fmt, opts) => Task.CompletedTask;
    
    /// <summary>
    /// The text of the message.
    /// </summary>
    public string? Text;
    /// <summary>
    /// The embeds of the message.
    /// </summary>
    public EmbedBuilder[]? Embeds = null;
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
    public EmbedBuilder? Embed;

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
    public DiscordFormat(EmbedBuilder embed)
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

    public delegate Task<IUserMessage> ModifyOriginalResponseAsyncDelegate(
        Action<MessageProperties> func,
        RequestOptions? options = null);
    
    public delegate Task<IUserMessage> SendChannelMessageAsyncDelegate(
        string? text = null,
        bool isTts = false,
        Embed? embed = null,
        RequestOptions? options = null,
        AllowedMentions? allowedMentions = null,
        MessageReference? messageReference = null,
        MessageComponent? components = null,
        ISticker[]? stickers = null,
        Embed[]? embeds = null);

    public delegate Task CustomDelegate(DiscordFormat format, SendOptions sendOptions);

    public async Task Send(RespondAsyncDelegate del, SendOptions sendOptions = default)
    {
        await del(Text, Embeds?.Select(e => e.Build()).ToArray(), sendOptions.IsTts, sendOptions.Ephemeral,
            AllowedMentions, sendOptions.Request, Components, Embed?.Build());
    }
    public async Task Send(SendMessageAsyncDelegate del, SendOptions sendOptions = default)
    {
        await del(Text, sendOptions.IsTts, Embed?.Build(), sendOptions.Request, AllowedMentions, Components,
            Embeds?.Select(e => e.Build()).ToArray());
    }
    public async Task Send(SendChannelMessageAsyncDelegate del, SendOptions sendOptions = default)
    {
        await del(Text, sendOptions.IsTts, Embed?.Build(), sendOptions.Request, AllowedMentions, null,
            Components, null, Embeds?.Select(e => e.Build()).ToArray());
    }
    public Task Send(ModifyOriginalResponseAsyncDelegate del, SendOptions sendOptions = default)
    {
        del(properties =>
        {
            properties.Content = Opt(Text);
            properties.Embed = Opt(Embed?.Build());
            properties.Embeds = Opt(Embeds?.Select(e => e.Build()).ToArray());
            properties.AllowedMentions = Opt(AllowedMentions);
            properties.Components = Opt(Components);
        }, sendOptions.Request);
        return Task.CompletedTask;
    }
    public Task Send(CustomDelegate del, SendOptions sendOptions = default)
    {
        return del(this, sendOptions);
    }

    private static Optional<T> Opt<T>(T? value)
    {
        return value == null ? Optional<T>.Unspecified : new Optional<T>(value);
    }

    /// <summary>
    /// Joins the given formats together.
    /// </summary>
    /// <param name="formats">The formats.</param>
    /// <returns>The joined formats.</returns>
    public static IEnumerable<DiscordFormat> Join(IEnumerable<DiscordFormat> formats)
    {
        var result = new List<DiscordFormat> { new() };
        var hasAny = false;
        foreach (var format in formats)
        {
            // Text
            if (!string.IsNullOrWhiteSpace(format.Text))
            {
                var newText = (result.Last().Text ?? "") + format.Text;
                if (newText.Length > 2000)
                {
                    result.Add(new DiscordFormat());
                    result.Last().Text = format.Text;
                }
                else
                {
                    result.Last().Text = newText;
                }
                hasAny = true;
            }
            // Embeds
            var embeds = format.Embeds;
            if (format.Embed != null)
            {
                embeds = new[] { format.Embed };
            }
            if (embeds != null)
            {
                var oldEmbeds = result.Last().Embeds;
                var newEmbeds = oldEmbeds != null 
                    ? oldEmbeds.Concat(embeds).ToArray() 
                    : embeds;
                if (newEmbeds.Length > 25)
                {
                    result.Add(new DiscordFormat());
                    result.Last().Embeds = embeds;
                }
                else
                {
                    result.Last().Embeds = newEmbeds;
                }
                hasAny = true;
            }
            // AllowedMentions
            if (format.AllowedMentions != null)
            {
                throw new InvalidOperationException("Cannot (yet) join DiscordFormats with AllowedMentions.");
            }
            // Components
            if (format.Components != null)
            {
                throw new InvalidOperationException("Cannot (yet) join DiscordFormats with Components.");
            }
        }

        return hasAny ? result : Array.Empty<DiscordFormat>();
    }
}
