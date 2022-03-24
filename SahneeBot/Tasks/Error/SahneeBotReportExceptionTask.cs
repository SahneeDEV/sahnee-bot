using System.Text;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBot.Formatter.Error;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks.Error;

/// <summary>
/// This task is used to report an error to the webhook and guild admins. Returns the ticket ID.
/// </summary>
public class SahneeBotReportExceptionTask : ITask<SahneeBotReportExceptionTask.Args, string>
{
    /// <summary>
    /// Arguments for the task.
    /// </summary>
    /// <param name="InteractionType">The type of interaction.</param>
    /// <param name="InteractionName">The name of the command that was executed.</param>
    /// <param name="FullInteraction">The full command string.</param>
    /// <param name="GuildId">The guild the command was executed on. null if global command.</param>
    /// <param name="UserId">The user that executed the command.</param>
    /// <param name="Exception">The actual error.</param>
    public record struct Args(string InteractionType, string InteractionName, string FullInteraction, ulong? GuildId
        , ulong? UserId, Exception Exception);
    
    private static readonly Random Rng = new();
    private readonly ILogger<SahneeBotReportExceptionTask> _logger;
    private readonly Bot _bot;
    private readonly ErrorWebhook _webhook;
    private readonly ExceptionDiscordFormatter _exceptionFmt;
    private readonly SahneeBotPrivateMessageToGuildMembersTask _privateMessage;

    public SahneeBotReportExceptionTask(ILogger<SahneeBotReportExceptionTask> logger
        , Bot bot
        , ErrorWebhook webhook
        , ExceptionDiscordFormatter exceptionFmt
        , SahneeBotPrivateMessageToGuildMembersTask privateMessage)
    {
        _logger = logger;
        _bot = bot;
        _webhook = webhook;
        _exceptionFmt = exceptionFmt;
        _privateMessage = privateMessage;
    }

    public async Task<string> Execute(ITaskContext ctx, Args arg)
    {
        var (interactionType, interactionName, fullInteraction, guildId, userId
            , exception) = arg;
        // Create ticket ID & get guild + user
        var ticketId = GenerateTicketId();
        var guild = guildId.HasValue ? await _bot.Client.GetGuildAsync(guildId.Value) : null;
        var user = userId.HasValue ? await _bot.Client.GetUserAsync(userId.Value) : null;
        // Write log
        _logger.LogError(EventIds.Command, exception, 
            "[TICKET #{TicketId}] Reported error in {Interaction} {InteractionType}\n  Guild: {Guild} " +
            "(#{GuildId})\n  User: {User}#{Discriminator} (#{UserId})\n  Interaction: {FullInteraction}"
            , ticketId, interactionName, interactionType, guild?.Name, guildId, user?.Username
            , user?.Discriminator, userId, fullInteraction);
        // Send to webhook
        if (_webhook.Client != null)
        {
            try
            {
                await _exceptionFmt.FormatAndSend(new ExceptionDiscordFormatter.Args(interactionType, interactionName,
                    fullInteraction, guildId, userId, exception, ticketId, true),
                    _webhook.Client.SendMessageAsync);
            }
            catch (Exception webhookError)
            {
                _logger.LogError(EventIds.Command, webhookError, "Failed to send message to error webhook");
            }
        }

        if (guildId.HasValue)
        {
            var message = await _exceptionFmt.Format(new ExceptionDiscordFormatter.Args(interactionType
                , interactionName, fullInteraction, guildId, userId, exception, ticketId, false));
            await _privateMessage.Execute(ctx,
                new SahneeBotPrivateMessageToGuildMembersTask.Args(
                    guildId.Value,
                    async pmGuild =>
                    {
                        var users = await pmGuild.GetUsersAsync();
                        return users.Where(pmUser => pmUser.GuildPermissions.Administrator);
                    },
                    new[] {message}, false, true));
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