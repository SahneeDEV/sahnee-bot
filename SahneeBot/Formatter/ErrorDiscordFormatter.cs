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
    /// <param name="GuildId">The guild the command was executed on. null if global command.</param>
    /// <param name="UserId">The user that executed the command.</param>
    /// <param name="Error">The actual error.</param>
    /// <param name="TicketId">The ticket ID</param>
    /// <param name="ReportSensitive">Also report sensitive information?</param>
    public record struct Args(string InteractionType
        , string InteractionName
        , string FullInteraction
        , ulong? GuildId
        , ulong? UserId
        , Exception Error
        , string TicketId
        , bool ReportSensitive);
    
    private readonly DefaultFormatArguments _fmt;
    private readonly IConfiguration _cfg;
    private readonly DiscordSocketClient _bot;

    public ErrorDiscordFormatter(
        DefaultFormatArguments fmt,
        IConfiguration cfg,
        DiscordSocketClient bot
        )
    {
        _fmt = fmt;
        _cfg = cfg;
        _bot = bot;
    }
    
    public async Task<DiscordFormat> Format(Args arg)
    {
        var (interactionType, interactionName, fullInteraction, guildId, userId, error, ticketId, reportSensitive) = arg;
        var guild = guildId.HasValue ? _bot.GetGuild(guildId.Value) : null;
        var user = userId.HasValue ? await _bot.GetUserAsync(userId.Value) : null;
        var embed = _fmt.GetEmbed();
        var supportServer = _cfg["BotSettings:SupportServer"];
        if (reportSensitive)
        {
            embed.Title = $"Error in {interactionName} {interactionType.ToLowerInvariant()} on " + 
                          (guild != null ? _fmt.GetMention(guild) : "a global interaction");
            embed.Color = Color.DarkRed;
            embed.AddField("Ticket Id", ticketId + " (" + supportServer + ")", true);
            embed.AddField("User", user != null ? _fmt.GetMention(user) 
                                                  + " `(#" + user.Id + ")`" : "n/a", true);
            embed.AddField("Server", guild != null ? _fmt.GetMention(guild) 
                                                     + " `(#" + guild.Id + ")`" : "n/a", true);
            embed.AddField(interactionType, "`" + fullInteraction + "`");
            embed.AddField(error.GetType().Name, error.Message[..Math.Min(error.Message.Length, 1000)]);
            embed.AddField("Stack trace",
                "```\n" + error.StackTrace?[..Math.Min(error.StackTrace?.Length ?? 0, 1000)] + "\n```");
        }
        else
        {
            embed.Title = $"Error in {interactionName} {interactionType.ToLowerInvariant()}";
            embed.Color = Color.DarkRed;
            embed.AddField(interactionType, interactionName, true);
            embed.AddField("Support Server", supportServer, true);
            embed.AddField("Ticket Id", ticketId, true);
            embed.AddField("Sorry!", "The Sahnee-Bot encountered an error while processing your " +
                                     $"{interactionType.ToLowerInvariant()}! Please join our support server and post " +
                                     "a screenshot of this message or your ticket ID.");
        }
        return new DiscordFormat(embed);
    }
}