using Discord;
using Microsoft.Extensions.Configuration;

namespace SahneeBot.Formatter.Error;

/// <summary>
/// Used to format an exception in a context.
/// </summary>
public class ExceptionDiscordFormatter : IDiscordFormatter<ExceptionDiscordFormatter.Args>
{
    /// <summary>
    /// Arguments for the formatter.
    /// </summary>
    /// <param name="InteractionType">The type of interaction.</param>
    /// <param name="InteractionName">The name of the command that was executed.</param>
    /// <param name="GuildId">The guild the command was executed on. null if global command.</param>
    /// <param name="UserId">The user that executed the command.</param>
    /// <param name="Exception">The actual error.</param>
    /// <param name="TicketId">The ticket ID</param>
    /// <param name="ReportSensitive">Also report sensitive information?</param>
    public record struct Args(string InteractionType
        , string InteractionName
        , string FullInteraction
        , ulong? GuildId
        , ulong? UserId
        , Exception Exception
        , string TicketId
        , bool ReportSensitive);
    
    private readonly DefaultFormatArguments _fmt;
    private readonly IConfiguration _cfg;
    private readonly Bot _bot;

    public ExceptionDiscordFormatter(DefaultFormatArguments fmt
        , IConfiguration cfg
        , Bot bot)
    {
        _fmt = fmt;
        _cfg = cfg;
        _bot = bot;
    }
    
    public async Task<DiscordFormat> Format(Args arg)
    {
        var (interactionType, interactionName, fullInteraction
            , guildId, userId, exception, ticketId, reportSensitive) = arg;
        var guild = guildId.HasValue ? await _bot.Client.GetGuildAsync(guildId.Value) : null;
        var user = userId.HasValue ? await _bot.Client.GetUserAsync(userId.Value) : null;
        var embed = _fmt.GetEmbed();
        var supportServer = _cfg["BotSettings:SupportServer"];
        var timestamp = DateTime.UtcNow;
        if (reportSensitive)
        {
            embed.Title = $"Error in {interactionName} {interactionType.ToLowerInvariant()} on " + 
                          (guild != null ? _fmt.GetMention(guild) : "a global interaction");
            embed.Color = Color.DarkRed;
            embed.AddField("Ticket Id", ticketId + " (" + supportServer + ")", true);
            embed.AddField("User", _fmt.GetMention(user, reportSensitive), true);
            embed.AddField("Server", _fmt.GetMention(guild, reportSensitive), true);
            embed.AddField("Timestamp", timestamp, true);
            embed.AddField(interactionType, "`" + fullInteraction + "`", true);
            embed.AddField(exception.GetType().Name, exception.Message[..Math.Min(exception.Message.Length, 1000)], true);
            embed.AddField("Stack trace",
                "```\n" + exception.StackTrace?[..Math.Min(exception.StackTrace?.Length ?? 0, 1000)] + "\n```");
        }
        else
        {
            embed.Title = $"Error in {interactionName} {interactionType.ToLowerInvariant()}";
            embed.Color = Color.DarkRed;
            embed.AddField(interactionType, "`" + interactionName + "`", true);
            embed.AddField("Support Server", supportServer, true);
            embed.AddField("Ticket Id", ticketId, true);
            embed.AddField("User", _fmt.GetMention(user, reportSensitive), true);
            embed.AddField("Server", _fmt.GetMention(guild, reportSensitive), true);
            embed.AddField("Timestamp", timestamp, true);
            embed.AddField("Sorry!", "The Sahnee-Bot encountered an error while processing your " +
                                     $"{interactionType.ToLowerInvariant()}! Please join our support server and post " +
                                     "a screenshot of this message or your ticket ID.");
        }
        return new DiscordFormat(embed);
    }
}