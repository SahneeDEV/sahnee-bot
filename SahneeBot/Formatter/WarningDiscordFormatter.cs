using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using SahneeBotModel.Contract;

namespace SahneeBot.Formatter;

public class WarningDiscordFormatter: IDiscordFormatter<IWarning>
{
    private readonly DiscordSocketClient _bot;
    private readonly IConfiguration _configuration;

    public WarningDiscordFormatter(DiscordSocketClient bot, IConfiguration configuration)
    {
        _bot = bot;
        _configuration = configuration;
    }
    
    public Task<DiscordFormat> Format(IWarning arg)
    {
        var guild = _bot.GetGuild(arg.GuildId);
        var user = _bot.GetUser(arg.UserId);
        return Task.FromResult(new DiscordFormat
            {
                Embed = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "SahneeBot",
                        Url = !string.IsNullOrWhiteSpace(_configuration["Url"])
                        ? _configuration["Url"]
                        : "https://sahnee.dev/en/project/sahnee-bot/",
                        IconUrl = !string.IsNullOrWhiteSpace(_configuration["BotSettings:IconUrl"])
                            ? _configuration["IconUrl"] 
                            : "https://sahnee.dev/wp-content/uploads/2020/04/sahnee-bot-150x150.png"
                    },
                    Color = new Color(243,9,131),
                    Title = $":thumbsdown: {user.Username} has been warned",
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new()
                        {
                            Name = "Warned",
                            Value = $"<@{arg.UserId}>",
                            IsInline = true
                        },
                        new()
                        {
                            Name = "Warned by",
                            Value = $"<@{arg.IssuerUserId}>",
                            IsInline = true
                        },
                        new()
                        {
                            Name = "Warning Number",
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
                    },
                    Timestamp = arg.Time,
                    Footer = new EmbedFooterBuilder
                    {
                        Text = "proudly presented by sahnee.dev",
                        IconUrl = !string.IsNullOrWhiteSpace(_configuration["BotSettings:IconUrl"])
                            ? _configuration["IconUrl"] 
                            : "https://sahnee.dev/wp-content/uploads/2020/04/sahnee-bot-150x150.png"
                    }
                }.Build()
            });
    }
}