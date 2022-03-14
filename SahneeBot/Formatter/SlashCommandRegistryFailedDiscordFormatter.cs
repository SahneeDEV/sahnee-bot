
using Discord;

namespace SahneeBot.Formatter;

/// <summary>
/// Informs the user the slash commands failing to register
/// </summary>
public class SlashCommandRegistryFailedDiscordFormatter : IDiscordFormatter<SlashCommandRegistryFailedDiscordFormatter.Args>
{
    public record struct Args(ulong GuildId, string Hint);

    private readonly DefaultFormatArguments _fmt;
    private readonly Bot _bot;

    public SlashCommandRegistryFailedDiscordFormatter(DefaultFormatArguments fmt
        , Bot bot)
    {
        _fmt = fmt;
        _bot = bot;
    }

    public async Task<DiscordFormat> Format(Args arg)
    {
        var (guildId, hint) = arg;
        var guild = await _bot.Client.GetGuildAsync(guildId);
        var embed = _fmt.GetEmbed();
        embed.Title = "Failed to register slash-commands";
        embed.Color = Color.DarkRed;
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Server",
                Value = _fmt.GetMention(guild),
                IsInline = true
            },
            new()
            {
                Name = "Hint",
                Value = hint,
                IsInline = false
            }
        };
        return new DiscordFormat(embed);
    }
}