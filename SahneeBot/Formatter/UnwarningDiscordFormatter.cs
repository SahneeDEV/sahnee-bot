using Discord;
using Discord.WebSocket;
using SahneeBotModel.Contract;

namespace SahneeBot.Formatter;

public class UnwarningDiscordFormatter: IDiscordFormatter<IWarning>
{
    private readonly DiscordSocketClient _bot;
    private readonly DefaultFormatArguments _defaultFormatArguments;

    public UnwarningDiscordFormatter(DiscordSocketClient bot, DefaultFormatArguments defaultFormatArguments)
    {
        _bot = bot;
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(IWarning arg)
    {
        var guild = _bot.GetGuild(arg.GuildId);
        var user = _bot.GetUser(arg.UserId);
        var embed = _defaultFormatArguments.GetEmbed();
        
        embed.Title = $":heart: {user.Username} has been unwarned";
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Unwarned",
                Value = $"<@{arg.UserId}>",
                IsInline = true
            },
            new()
            {
                Name = "Unwarned by",
                Value = $"<@{arg.IssuerUserId}>",
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
                Value = $"{guild.Name}",
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

        return Task.FromResult(new DiscordFormat(embed.Build()));
    }
}