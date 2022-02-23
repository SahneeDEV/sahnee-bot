using System.Text;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SahneeBot.Formatter;

/// <summary>
/// Used to format an error in a command.
/// </summary>
public class ErrorDiscordFormatter : IDiscordFormatter<ErrorDiscordFormatter.Args>
{
    /// <summary>
    /// Arguments for the formatter.
    /// </summary>
    /// <param name="InteractionType">The type of interaction.</param>
    /// <param name="InteractionName">The name of the command that was executed.</param>
    /// <param name="FullInteraction">The full command string.</param>
    /// <param name="GuildId">The guild the command was executed on. null if global command.</param>
    /// <param name="UserId">The user that executed the command.</param>
    /// <param name="Error">The actual error.</param>
    public record struct Args(string InteractionType, string InteractionName, string FullInteraction, ulong? GuildId, ulong? UserId, Exception Error);
    
    private readonly DefaultFormatArguments _fmt;
    private readonly IConfiguration _cfg;
    private readonly ILogger<ErrorDiscordFormatter> _logger;
    private readonly DiscordSocketClient _bot;
    private static readonly Random Rng = new();

    public ErrorDiscordFormatter(
        DefaultFormatArguments fmt,
        IConfiguration cfg,
        ILogger<ErrorDiscordFormatter> logger,
        DiscordSocketClient bot
        )
    {
        _fmt = fmt;
        _cfg = cfg;
        _logger = logger;
        _bot = bot;
    }
    
    public async Task<DiscordFormat> Format(Args arg)
    {
        var (interactionType, commandName, fullCommand, guildId, userId, exception) = arg;
        var guild = guildId.HasValue ? _bot.GetGuild(guildId.Value) : null;
        var user = userId.HasValue ? await _bot.GetUserAsync(userId.Value) : null;
        var bytes = new byte[4];
        Rng.NextBytes(bytes);
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            sb.Append($"{b:X2}");
        }
        var ticketId = "SB-" + sb;
        var embed = _fmt.GetEmbed();
        var supportServer = _cfg["BotSettings:SupportServer"];
        embed.Title = $"Error in {commandName} {interactionType.ToLowerInvariant()}";
        embed.Color = Color.DarkRed;
        embed.AddField(interactionType, commandName, true);
        embed.AddField("Support Server", supportServer, true);
        embed.AddField("Ticket Id", ticketId, true);
        embed.AddField("Sorry!", $"The Sahnee-Bot encountered an error while processing your {interactionType.ToLowerInvariant()}! Please " +
                                 "join our support server and post a screenshot of this message or your ticket ID.");
        _logger.LogError(EventIds.Command, exception, 
            "[TICKET #{TicketId}] Reported error in {Command} {InteractionType}\n  Guild: {Guid} (#{GuildId})\n  User: {User}#{Discriminator} (#{UserId})\n  Interaction: {FullCommand}", 
            ticketId, commandName, interactionType, guild?.Name, guildId, user?.Username, user?.Discriminator, userId, fullCommand);
        return new DiscordFormat(embed);
    }
}