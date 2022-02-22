using Discord;

namespace SahneeBot.Formatter;

public class WelcomeOnNewGuildJoinDiscordFormatter : IDiscordFormatter<WelcomeOnNewGuildJoinDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    public record struct Args(string GuildName);
    
    public WelcomeOnNewGuildJoinDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args args)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = args.GuildName + " welcome to Sahnee-Bot";
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "How to use commands?",
                Value = "We are using the / commands (slash commands). Just give it a try and hit / in a channel",
                IsInline = false
            },
            new()
            {
                Name = "Want to bind commands to a specific channel?",
                Value = "You can bind commands to a specific channel with '/config bind <set/unset/get>'",
                IsInline = false
            },
            new()
            {
                Name = "Get started with warning someone",
                Value = "Type '/warn @username <your reason here>'",
                IsInline = false
            },
            new()
            {
                Name = "Limit access to commands",
                Value = "You can set permissions to roles on your guild via /config sahnee-permission <add/remove/list>",
                IsInline = false
            },
            new()
            {
                Name = "Don't want warning-roles to be set?",
                Value = "Type /config role <enable/disable/status>",
                IsInline = true
            },
            new()
            {
                Name = "Custom color for warning roles?",
                Value = "Use /config role color <color in #hex>",
                IsInline = true
            },
            new()
            {
                Name = "Explore the other commands",
                Value = "Just use the / and explore what else we got for you to manage your server",
                IsInline = false
            },
            new()
            {
                Name = "Privacy Policy",
                Value = "By using the Sahnee-Bot you agree to our" +
                        " [privacy policy](https://sahnee.dev/en/sahnee-bot-privacy-policy/).",
                IsInline = false
            }
        };
        
        return Task.FromResult(new DiscordFormat(embed));
    }
}