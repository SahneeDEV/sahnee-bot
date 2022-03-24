using Discord;
using Microsoft.Extensions.Configuration;
using SahneeBotController;

namespace SahneeBot.Formatter.Error;

/// <summary>
/// Used to report an error (not an exception/crash) in a context.
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
    public record struct Args(string InteractionType
        , string InteractionName
        , string FullInteraction
        , ulong? GuildId
        , ulong? UserId
        , ISuccess Error
        , bool ReportSensitive);

    private readonly DefaultFormatArguments _fmt;
    private readonly Bot _bot;
    private readonly IConfiguration _cfg;

    public ErrorDiscordFormatter(DefaultFormatArguments fmt
        , Bot bot
        , IConfiguration cfg)
    {
        _fmt = fmt;
        _bot = bot;
        _cfg = cfg;
    }

    public async Task<DiscordFormat> Format(Args arg)
    {
        var (interactionType, interactionName, fullInteraction, guildId, userId
            , error, reportSensitive) = arg;
        var embed = _fmt.GetEmbed();
        var guild = guildId.HasValue ? await _bot.Client.GetGuildAsync(guildId.Value) : null;
        var user = userId.HasValue ? await _bot.Client.GetUserAsync(userId.Value) : null;
        var supportServer = _cfg["BotSettings:SupportServer"];
        embed.Title = $"Failed to execute {interactionName} {interactionType}";
        embed.Color = Color.DarkRed;
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Server",
                Value = _fmt.GetMention(guild),
                IsInline = true,
            },
            new()
            {
                Name = "User",
                Value = _fmt.GetMention(user),
                IsInline = true,
            },
            new()
            {
                Name = "Support Server",
                Value = supportServer,
                IsInline = true
            },
        };

        if (reportSensitive)
        {
            embed.Fields.Add(
                new EmbedFieldBuilder
                {
                    Name = interactionType,
                    Value = fullInteraction,
                    IsInline = false
                });
        }
        
        embed.Fields.Add(
            new EmbedFieldBuilder
            {
                Name = "Hint",
                Value = error.Message,
                IsInline = false
            });

        return new DiscordFormat(embed);
    }
}