using Discord;
using Discord.WebSocket;
using SahneeBotModel;
using IRole = SahneeBotModel.Contract.IRole;

namespace SahneeBot.Formatter;

public class RoleDiscordFormatter : IDiscordFormatter<IRole>
{
    private static readonly RoleType[] Values = Enum.GetValues<RoleType>();
    private static readonly RoleType[] None = { RoleType.None };
    
    private readonly DefaultFormatArguments _fmt;
    private readonly Bot _bot;

    public RoleDiscordFormatter(DefaultFormatArguments fmt
        , Bot bot)
    {
        _fmt = fmt;
        _bot = bot;
    }
    
    public async Task<DiscordFormat> Format(IRole arg)
    {
        var guild = await _bot.Client.GetGuildAsync(arg.GuildId);
        var discordRole = guild.GetRole(arg.RoleId);
        
        if (discordRole == null)
        {
            return new DiscordFormat("The role does not exist.");
        }
        
        var builder = _fmt.GetEmbed();
        var roles = RolesIn(arg.RoleType).ToArray();
        builder.Title = $"Sahnee permissions of role \"{discordRole.Name}\"";
        builder.Fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Role",
                Value = _fmt.GetMention(discordRole),
                IsInline = true
            },
            new()
            {
                Name = roles.Length == 1 ? "Sahnee permission" : "Sahnee permissions",
                Value = string.Join(", ", roles),
                IsInline = true
            },
            new()
            {
                Name = "Server",
                Value = guild.Name,
                IsInline = true
            },
        };

        return new DiscordFormat(builder);
    }
    
    private static IEnumerable<RoleType> RolesIn(RoleType roleType)
    {
        var roles = Values
            .Where(value => (roleType & value) == value && value != RoleType.None)
            .ToArray();
        return roles.Length == 0 ? None : roles;
    }
}