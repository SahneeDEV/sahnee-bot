using Discord;
using Discord.WebSocket;
using SahneeBotModel;
using SahneeBotModel.Contract;

namespace SahneeBot.Formatter;

public class WarningDiscordFormatter: IDiscordFormatter<IWarning>
{
    private readonly DiscordSocketClient _bot;
    private readonly DefaultFormatArguments _fmt;

    public WarningDiscordFormatter(DiscordSocketClient bot, DefaultFormatArguments fmt)
    {
        _bot = bot;
        _fmt = fmt;
    }
    
    public async Task<DiscordFormat> Format(IWarning arg)
    {
        var guild = _bot.GetGuild(arg.GuildId);
        var user = await _bot.GetUserAsync(arg.UserId);
        var issuer = await _bot.GetUserAsync(arg.IssuerUserId);
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
                Value = user != null ? _fmt.GetMention(user) : "n/a",
                IsInline = true
            },
            new()
            {
                Name = unwarn ? "Unwarned by" : "Warned by",
                Value = issuer != null ? _fmt.GetMention(issuer) : "n/a",
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
                Value = guild != null ? _fmt.GetMention(guild) : "n/a",
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