using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// Prints a welcome message on a new guild.
/// </summary>
public class WelcomeOnNewGuildJoinDiscordFormatter : IDiscordFormatter<WelcomeOnNewGuildJoinDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    /// <summary>
    /// Formatter arguments.
    /// </summary>
    /// <param name="GuildName">The name of the guild the bot just joined.</param>
    public record struct Args(string GuildName);
    
    public WelcomeOnNewGuildJoinDiscordFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args args)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = "Welcome to Sahnee-Bot, " + args.GuildName;
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "How to use commands?",
                Value = "We are using the `/` commands (slash commands). Just give it a try and hit `/` in a channel",
                IsInline = false
            },
            new()
            {
                Name = "Want to bind commands to a specific channel?",
                Value = "You can bind commands to a specific channel with `/config bind <set/unset/get>`",
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
                Value = "You can set permissions to roles on your guild via `/config sahnee-permission " +
                        "<add/remove/list>`. By default administrators have full access the the bot and users that " +
                        "have the permission to ban others can issue warnings.",
                IsInline = false
            },
            new()
            {
                Name = "Don't want warning roles to be set?",
                Value = "Type `/config role <enable/disable/status>`",
                IsInline = true
            },
            new()
            {
                Name = "Custom color for warning roles?",
                Value = "Use `/config role color <color in #hex>`",
                IsInline = true
            },
            new()
            {
                Name = "Explore the other commands",
                Value = "Just use the `/` and explore what else we got for you to manage your server",
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