using Discord;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotSendWarningMessageToUserTask : SendWarningMessageToUserTask
{
    private readonly ILogger<SahneeBotSendWarningMessageToUserTask> _logger;
    private readonly WarningDiscordFormatter _discordFormatter;
    private readonly SahneeBotPrivateMessageToGuildMembersTask _privateMessage;

    public SahneeBotSendWarningMessageToUserTask(IServiceProvider provider
        , ILogger<SahneeBotSendWarningMessageToUserTask> logger
        , WarningDiscordFormatter discordFormatter
        , SahneeBotPrivateMessageToGuildMembersTask privateMessage) : base(provider)
    {
        _logger = logger;
        _discordFormatter = discordFormatter;
        _privateMessage = privateMessage;
    }

    protected override async Task<bool> ExecuteImpl(ITaskContext ctx, Args arg)
    {
        var warning = arg.Warning;
        var message = await _discordFormatter.Format(warning);
        var recipients = await _privateMessage.Execute(ctx,
            new SahneeBotPrivateMessageToGuildMembersTask.Args(
                arg.Warning.GuildId,
                async guild =>
                {
                    var user = await guild.GetGuildUserAsync(warning.UserId);
                    if (user != null)
                    {
                        return new[] {user};
                    }
                    _logger.LogWarning(EventIds.Discord,
                        "Failed to deliver warning message for warning {Warning}: Could not get the user", 
                        warning);
                    return Array.Empty<IUser>();
                }, new[] {message}));
        return recipients.IsSuccess && recipients.Value == 1;
    }
}