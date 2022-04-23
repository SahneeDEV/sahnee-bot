using Discord.WebSocket;

namespace SahneeBot.Formatter;

public class RemovedUsersFromGuildStateDiscordFormatter 
    : IDiscordFormatter<RemovedUsersFromGuildStateDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;
    private readonly Bot _bot;

    /// <summary>
    /// Arguments for the formatter
    /// </summary>
    /// <param name="GuildId"></param>
    /// <param name="AmountRemoved"></param>
    public record struct Args(ulong GuildId, int AmountRemoved, int Failed);

    public RemovedUsersFromGuildStateDiscordFormatter(DefaultFormatArguments defaultFormatArguments
        , Bot bot)
    {
        _defaultFormatArguments = defaultFormatArguments;
        _bot = bot;
    }
    
    public async Task<DiscordFormat> Format(Args arg)
    {
        var (guildId, amountRemoved, failed) = arg;
        var embed = _defaultFormatArguments.GetEmbed();
        var guild = await _bot.Client.GetGuildAsync(guildId);
        embed.Title = amountRemoved > 0 ? "Removed users from the Database" 
            : failed != 0 ? "Could not Remove some users" 
            : "No users to remove";
        embed.AddField("In Server", _defaultFormatArguments.GetMention(guild), true);
        embed.AddField("Amount removed", amountRemoved.ToString(), true);
        if (failed > 0)
        {
            embed.AddField("Failed amount",failed,true);
        }
        
        return new DiscordFormat(embed);
    }
}