using Discord;
using Microsoft.Extensions.Configuration;

namespace SahneeBot.Formatter;

/// <summary>
/// Provides the default structures of the messages
/// </summary>
public class DefaultFormatArguments
{
    private readonly string _defaultName = "SahneeBot";
    private readonly string _defaultWebsiteUrl = "https://sahnee.dev/en/project/sahnee-bot/";
    private readonly string _defaultIconUrl = "https://sahnee.dev/wp-content/uploads/2020/04/sahnee-bot-150x150.png";
    private readonly string _defaultFooterText = "proudly presented by sahnee.dev";
    private readonly Color _color = new Color(243, 9, 131);

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
            Name = _defaultName,
            Url = !string.IsNullOrWhiteSpace(_configuration["BotSettings:Url"])
                ? _configuration["BotSettings:Url"]
                : _defaultWebsiteUrl,
            IconUrl = !string.IsNullOrWhiteSpace(_configuration["BotSettings:IconUrl"])
                ? _configuration["BotSettings:IconUrl"]
                : _defaultIconUrl
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
            Text = _defaultFooterText,
            IconUrl = !string.IsNullOrWhiteSpace(_configuration["BotSettings:IconUrl"])
                ? _configuration["BotSettings:IconUrl"]
                : _defaultIconUrl
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