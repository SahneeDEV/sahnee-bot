using Discord;

namespace SahneeBot.Formatter;

public class PrivateMessageToGuildOwnerFormatter : IDiscordFormatter<PrivateMessageToGuildOwnerFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;

    /// <summary>
    /// Arguments for printing the warning to the guild owner
    /// </summary>
    /// <param name="GuildName"></param>
    public record struct Args(string GuildName, string Title, string MessageName, string MessageValue);

    public PrivateMessageToGuildOwnerFormatter(DefaultFormatArguments defaultFormatArguments)
    {
        _defaultFormatArguments = defaultFormatArguments;
    }

    public Task<DiscordFormat> Format(Args args)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = args.Title;
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = args.MessageName,
                Value = args.MessageValue,
                IsInline = false
            }
        };
        return Task.FromResult(new DiscordFormat(embed));
    }
}