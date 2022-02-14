using Discord;
using Discord.WebSocket;
using IRole = SahneeBotModel.Contract.IRole;

namespace SahneeBot.Formatter;

/// <summary>
/// Formats a role.
/// </summary>
public class RoleDiscordFormatter : IDiscordFormatter<IRole?>
{
    private readonly DefaultFormatArguments _fmt;
    private readonly DiscordSocketClient _bot;

    public RoleDiscordFormatter(DefaultFormatArguments fmt, DiscordSocketClient bot)
    {
        _fmt = fmt;
        _bot = bot;
    }
    
    public Task<DiscordFormat> Format(IRole? arg)
    {
        if (arg == null)
        {
            return Task.FromResult(new DiscordFormat("The role does not exist."));
        }
        var builder = _fmt.GetEmbed();
        var guild = _bot.GetGuild(arg.GuildId);
        builder.Title = $"Bot permission role {arg.RoleName}";
        builder.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Role name",
                Value = arg.RoleName,
                IsInline = true
            },
            new()
            {
                Name = "Role type",
                Value = arg.RoleType,
                IsInline = true
            },
            new()
            {
                Name = "Server",
                Value = guild.Name,
                IsInline = true
            },
        };

        return Task.FromResult(new DiscordFormat(builder.Build()));
    }
}