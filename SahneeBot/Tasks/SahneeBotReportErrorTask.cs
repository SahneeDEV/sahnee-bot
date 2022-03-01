using System.Text;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

/// <summary>
/// This task is used to report an error. Returns the ticket ID.
/// </summary>
public class SahneeBotReportErrorTask : ITask<SahneeBotReportErrorTask.Args, string>
{
    /// <summary>
    /// Arguments for the task.
    /// </summary>
    /// <param name="InteractionType">The type of interaction.</param>
    /// <param name="InteractionName">The name of the command that was executed.</param>
    /// <param name="FullInteraction">The full command string.</param>
    /// <param name="GuildId">The guild the command was executed on. null if global command.</param>
    /// <param name="UserId">The user that executed the command.</param>
    /// <param name="Error">The actual error.</param>
    public record struct Args(string InteractionType, string InteractionName, string FullInteraction, ulong? GuildId, ulong? UserId, Exception Error);
    
    private static readonly Random Rng = new();
    private readonly ILogger<SahneeBotReportErrorTask> _logger;
    private readonly DiscordSocketClient _bot;
    private readonly ErrorWebhook _webhook;
    private readonly ErrorDiscordFormatter _webhookFmt;

    public SahneeBotReportErrorTask(
        ILogger<SahneeBotReportErrorTask> logger,
        DiscordSocketClient bot,
        ErrorWebhook webhook,
        ErrorDiscordFormatter webhookFmt)
    {
        _logger = logger;
        _bot = bot;
        _webhook = webhook;
        _webhookFmt = webhookFmt;
    }

    public async Task<string> Execute(ITaskContext ctx, Args arg)
    {
        var (interactionType, interactionName, fullInteraction, guildId, userId, exception) = arg;
        // Create ticket ID & get guild + user
        var ticketId = GenerateTicketId();
        var guild = guildId.HasValue ? _bot.GetGuild(guildId.Value) : null;
        var user = userId.HasValue ? await _bot.GetUserAsync(userId.Value) : null;
        // Write log
        _logger.LogError(EventIds.Command, exception, 
            "[TICKET #{TicketId}] Reported error in {Interaction} {InteractionType}\n  Guild: {Guild} (#{GuildId})\n  User: {User}#{Discriminator} (#{UserId})\n  Interaction: {FullInteraction}", 
            ticketId, interactionName, interactionType, guild?.Name, guildId, user?.Username, user?.Discriminator, userId, fullInteraction);
        // Send to webhook
        if (_webhook.Client != null)
        {
            try
            {
                await _webhookFmt.FormatAndSend(new ErrorDiscordFormatter.Args(interactionType, interactionName,
                    fullInteraction, guildId, userId, exception, ticketId, true),
                    _webhook.Client.SendMessageAsync);
            }
            catch (Exception webhookError)
            {
                _logger.LogError(EventIds.Command, webhookError, "Failed to send message to error webhook");
            }
        }

        return ticketId;
    }

    private static string GenerateTicketId()
    {
        var bytes = new byte[4];
        Rng.NextBytes(bytes);
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            sb.Append($"{b:X2}");
        }
        var ticketId = "SB-" + sb;
        return ticketId;
    }
}