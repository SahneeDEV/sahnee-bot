using Discord.WebSocket;

namespace SahneeBot.Formatter;

public class RemovedUsersFromGuildStateDiscordFormatter 
    : IDiscordFormatter<RemovedUsersFromGuildStateDiscordFormatter.Args>
{
    private readonly DefaultFormatArguments _defaultFormatArguments;
    private readonly DiscordSocketClient _bot;

    /// <summary>
    /// Arguments for the formatter
    /// </summary>
    /// <param name="GuildId"></param>
    /// <param name="AmountRemoved"></param>
    public record struct Args(ulong GuildId, int AmountRemoved, int Failed);

    public RemovedUsersFromGuildStateDiscordFormatter(DefaultFormatArguments defaultFormatArguments
    , DiscordSocketClient bot)
    {
        _defaultFormatArguments = defaultFormatArguments;
        _bot = bot;
    }
    
    public Task<DiscordFormat> Format(Args arg)
    {
        var embed = _defaultFormatArguments.GetEmbed();
        embed.Title = arg.AmountRemoved > 0 ? "Removed users from the Database" 
            : arg.Failed != 0 ? "Could not Remove some users" 
            : "No users to remove";
        embed.AddField("In Server", _bot.GetGuild(arg.GuildId).Name, true);
        embed.AddField("Amount removed", arg.AmountRemoved.ToString(), true);
        if (arg.Failed > 0)
        {
            embed.AddField("Failed amount",arg.Failed,true);
        }
        
        return Task.FromResult(new DiscordFormat(embed));
    }
}