using Discord;
using Discord.WebSocket;

namespace SahneeBot.Formatter;

/// <summary>
/// Formats that a job failed.
/// </summary>
public class JobFailedDiscordFormatter : IDiscordFormatter<JobFailedDiscordFormatter.Args>
{
    private readonly DiscordSocketClient _bot;
    private readonly DefaultFormatArguments _fmt;

    /// <summary>
    /// Arguments for the formatter.
    /// </summary>
    /// <param name="GuildId">The guild on which the job failed.</param>
    /// <param name="JobName">The name of the job that failed.</param>
    /// <param name="Hint">Why the job failed.</param>
    public record struct Args(ulong GuildId, string JobName, string Hint);
    
    public JobFailedDiscordFormatter(DiscordSocketClient bot
        , DefaultFormatArguments fmt)
    {
        _bot = bot;
        _fmt = fmt;
    }

    public Task<DiscordFormat> Format(Args arg)
    {
        var (guildId, jobName, hint) = arg;
        var guild = _bot.GetGuild(guildId);
        var guildStr = guild != null ? _fmt.GetMention(guild) : "n/a";
        var embed = _fmt.GetEmbed();
        embed.Title = "Background " + jobName + " failed";
        embed.Color = Color.DarkRed;
        embed.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Server",
                Value = guildStr,
                IsInline = true
            },
            new()
            {
                Name = "Hint",
                Value = hint,
                IsInline = false
            }
        };

        return Task.FromResult(new DiscordFormat(embed));
    }
}