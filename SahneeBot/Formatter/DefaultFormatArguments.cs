using System.Diagnostics.CodeAnalysis;
using Discord;
using Microsoft.Extensions.Configuration;

namespace SahneeBot.Formatter;

/// <summary>
/// Provides the default structures of the messages
/// </summary>
public class DefaultFormatArguments
{
    private const string DEFAULT_NAME = "SahneeBot";
    private const string DEFAULT_WEBSITE_URL = "https://sahnee.dev/en/project/sahnee-bot/";
    private const string DEFAULT_ICON_URL = "https://sahnee.dev/wp-content/uploads/2020/04/sahnee-bot-150x150.png";
    private const string DEFAULT_FOOTER_TEXT = "proudly presented by sahnee.dev";
    private readonly Color _color = new(243, 9, 131);

    private readonly IConfiguration _configuration;
    
    public DefaultFormatArguments(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generates the author for an embed message
    /// </summary>
    /// <returns>An EmbedAuthorBuilder with the standard content</returns>
    public EmbedAuthorBuilder GetAuthor()
    {
        return new EmbedAuthorBuilder()
        {
            Name = DEFAULT_NAME,
            Url = !string.IsNullOrWhiteSpace(_configuration["BotSettings:Url"])
                ? _configuration["BotSettings:Url"]
                : DEFAULT_WEBSITE_URL,
            IconUrl = !string.IsNullOrWhiteSpace(_configuration["BotSettings:IconUrl"])
                ? _configuration["BotSettings:IconUrl"]
                : DEFAULT_ICON_URL
        };
    }

    /// <summary>
    /// Generates a footer for an embed message
    /// </summary>
    /// <returns>An EmbedFooterBuilder with the standard content</returns>
    public EmbedFooterBuilder GetFooter()
    {
        return new EmbedFooterBuilder()
        {
            Text = DEFAULT_FOOTER_TEXT,
            IconUrl = !string.IsNullOrWhiteSpace(_configuration["BotSettings:IconUrl"])
                ? _configuration["BotSettings:IconUrl"]
                : DEFAULT_ICON_URL
        };
    }
    
    /// <summary>
    /// Generates the default color for an embed message
    /// </summary>
    /// <returns>A color</returns>
    public Color GetColor()
    {
        return _color;
    }

    /// <summary>
    /// Mentions the given user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="sensitive">Include sensitive information?</param>
    /// <returns>The mention.</returns>
    // ReSharper disable MemberCanBeMadeStatic.Global
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public string GetMention(IUser? user, bool sensitive = false)
    {
        return user == null ? "n/a" : user.Mention + (sensitive ? " (#" + user.Id + ")" : "");
    }
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public string GetMention(IGuild? guild, bool sensitive = false)
    {
        return guild == null ? "n/a" : $"*{guild.Name}*" + (sensitive ? " (#" + guild.Id + ")" : "");
    }
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public string GetMention(IRole? role, bool sensitive = false)
    {
        return role == null ? "n/a" : role.Mention + (sensitive ? " (#" + role.Id + ")" : "");
    }
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public string GetMention(ulong? userId, bool sensitive = false)
    {
        if (!userId.HasValue)
        {
            return "n/a";
        }
        return "<@" + userId.Value + ">" + (sensitive ? " (#" + userId + ")" : "");
    }
    // ReSharper enable MemberCanBeMadeStatic.Global

    /// <summary>
    /// Generates a finished embed with author, color and footer prefilled
    /// </summary>
    /// <returns>Ane EmbedBuilder with author, color and footer prefilled</returns>
    public EmbedBuilder GetEmbed()
    {
        return new EmbedBuilder()
        {
            Author = GetAuthor(),
            Color = GetColor(),
            Footer = GetFooter()
        };
    }
}