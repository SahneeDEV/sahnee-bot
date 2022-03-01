using Discord.Webhook;

namespace SahneeBot;

/// <summary>
/// The error webhook used to report errors.
/// </summary>
/// <param name="Client">The webhook client.</param>
public record ErrorWebhook(DiscordWebhookClient? Client);