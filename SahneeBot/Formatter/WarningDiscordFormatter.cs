using Discord;
using Discord.WebSocket;
using SahneeBotModel;
using SahneeBotModel.Contract;

namespace SahneeBot.Formatter;

public class WarningDiscordFormatter: IDiscordFormatter<IWarning>
{
    private readonly Bot _bot;
    private readonly DefaultFormatArguments _fmt;

    public WarningDiscordFormatter(Bot bot
        , DefaultFormatArguments fmt)
    {
        _bot = bot;
        _fmt = fmt;
    }
    
    public async Task<DiscordFormat> Format(IWarning arg)
    {
        var guild = await _bot.Client.GetGuildAsync(arg.GuildId);
        var user = await _bot.Client.GetUserAsync(arg.UserId);
        var issuer = await _bot.Client.GetUserAsync(arg.IssuerUserId);
        var embed = _fmt.GetEmbed();
        var unwarn = arg.Type == WarningType.Unwarning;

        embed.Title = unwarn
            ? $":heart: {user?.Username ?? "n/a"} has been unwarned"
            : $":thumbsdown: {user?.Username ?? "n/a"} has been warned";
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = unwarn ? "Unwarned" : "Warned",
                Value = _fmt.GetMention(user),
                IsInline = true
            },
            new()
            {
                Name = unwarn ? "Unwarned by" : "Warned by",
                Value = _fmt.GetMention(issuer),
                IsInline = true
            },
            new()
            {
                Name = "Warning Count",
                Value = $"{arg.Number}",
                IsInline = true
            },
            new()
            {
                Name = "In Server",
                Value = _fmt.GetMention(guild),
                IsInline = true
            },
            new()
            {
                Name = "Message",
                Value = arg.Reason,
                IsInline = false
            }
        };
        embed.Timestamp = arg.Time;

        return new DiscordFormat(embed);
    }
}