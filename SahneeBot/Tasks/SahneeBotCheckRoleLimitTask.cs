using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotCheckRoleLimitTask : CheckRoleLimitTask
{
    private readonly InformRoleLimitDiscordFormatter _informRoleLimitDiscordFormatter;
    private readonly Bot _bot;
    private readonly SahneeBotPrivateMessageToGuildMembersTask _privateMessage;
    private readonly ILogger<SahneeBotCheckRoleLimitTask> _logger;

    public SahneeBotCheckRoleLimitTask(IServiceProvider provider
        , InformRoleLimitDiscordFormatter informRoleLimitDiscordFormatter
        , Bot bot
        , SahneeBotPrivateMessageToGuildMembersTask privateMessage
        , ILogger<SahneeBotCheckRoleLimitTask> logger) : base(provider)
    {
        _informRoleLimitDiscordFormatter = informRoleLimitDiscordFormatter;
        _bot = bot;
        _privateMessage = privateMessage;
        _logger = logger;
    }

    protected override async Task<uint> GetRoleCount(ITaskContext ctx, ulong guildId)
    {
        // Guild not found?
        var guild = await _bot.Client.GetGuildAsync(guildId);
        if (guild == null)
        {
            return 0;
        }

        // Still enough role slots
        return (uint) guild.Roles.Count;
    }

    protected override async Task SendWarning(ITaskContext ctx, ulong guildId, uint roleCount)
    {
        var message = await _informRoleLimitDiscordFormatter.Format(
            new InformRoleLimitDiscordFormatter.Args((int)roleCount));
        var pmSuccess = await _privateMessage.Execute(ctx, 
            new SahneeBotPrivateMessageToGuildMembersTask.Args(
                guildId,
                async guild =>
                {
                    var users = await guild.GetUsersAsync();
                    return users.Where(user => user.GuildPermissions.Administrator);
                },
                new [] {message}));
        if (!pmSuccess.IsSuccess)
        {
            _logger.LogWarning(EventIds.Task
                , "Could not inform admins of guild {Guild} about their role limit of {Roles}: {Error}"
                , guildId, roleCount, pmSuccess.Message);
        }
    }
}
