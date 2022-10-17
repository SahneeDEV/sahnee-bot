using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SahneeBotController.Tasks.Changelog;

namespace SahneeBot.Jobs;

/// <summary>
/// This job sends changelogs to guilds that do not have them yet in regular intervals.
/// </summary>
[Job]
public sealed class SendChangelogsJob : JobBase {
    private readonly ILogger<SendChangelogsJob> _logger;
    private readonly Bot _bot;
    private readonly UpdateGuildChangelogTask _updateGuildChangelogTask;
    private readonly uint _maxChangelogsSendCount;

    public SendChangelogsJob(IServiceProvider serviceProvider
        , ILogger<SendChangelogsJob> logger
        , IConfiguration cfg
        , Bot bot
        , UpdateGuildChangelogTask updateGuildChangelogTask) : base(serviceProvider) {
        _logger = logger;
        _bot = bot;
        _updateGuildChangelogTask = updateGuildChangelogTask;
        _maxChangelogsSendCount = uint.Parse(cfg["BotSettings:MaxChangelogsSendCount"]);
        Time = new JobTimeSpanRepeat(TimeSpan.Parse(cfg["BotSettings:Jobs:UpdateGuildChangelog"]));
    }

    public override async Task Perform() {
        _logger.LogDebug(EventIds.Jobs, "Starting changelogs job");

        uint sentChangelogs = 0;
        
        // go through all guilds
        foreach (var currentGuild in await _bot.Client.GetGuildsAsync()) {
            var guild = currentGuild;
            if (guild == null) {
                continue;
            }
            
            // try to send new changelogs for each guild
            await PerformAsync(async ctx => {
                var res = await _updateGuildChangelogTask.Execute(ctx
                    , new UpdateGuildChangelogTask.Args(guild.Id));
                // track the amount of changelogs sent
                if (res.IsSuccess && res.Value.DidSendChangelog) {
                    sentChangelogs++;
                    _logger.LogDebug("... now sent {Changelogs} changelogs", sentChangelogs);
                }
                return res;
            }, new JobExecutionOptions {
                // PlaceInQueue = guild.Id // no queue to make awaitable with the result
                RelatedGuildId = guild.Id
                , Name = "send new changelogs"
                , Debug = guild.Id.ToString()
            });

            // stop if we have sent enough, do the next next time
            if (sentChangelogs >= _maxChangelogsSendCount) {
                break;
            }
        }
    }
}