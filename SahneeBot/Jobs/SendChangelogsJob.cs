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
    private readonly GuildChangelogQueue _queue;
    private readonly Bot _bot;
    private readonly UpdateGuildChangelogTask _updateGuildChangelogTask;
    private readonly uint _maxChangelogsSendCount;

    public SendChangelogsJob(IServiceProvider serviceProvider
        , ILogger<SendChangelogsJob> logger
        , GuildChangelogQueue queue
        , IConfiguration cfg
        , Bot bot
        , UpdateGuildChangelogTask updateGuildChangelogTask) : base(serviceProvider) {
        _logger = logger;
        _queue = queue;
        _bot = bot;
        _updateGuildChangelogTask = updateGuildChangelogTask;
        _maxChangelogsSendCount = uint.Parse(cfg["BotSettings:MaxChangelogsSendCount"]);
        Time = new JobTimeSpanRepeat(TimeSpan.Parse(cfg["BotSettings:Jobs:UpdateGuildChangelog"]));
    }

    public override async Task Perform() {
        _logger.LogDebug(EventIds.Jobs, "Starting changelogs job");
        
        uint sentChangelogs = 0;
        while (sentChangelogs < _maxChangelogsSendCount && _queue.TryDequeue(out var guildId))
        {
            var guild = await _bot.Client.GetGuildAsync(guildId);
            if (guild == null)
            {
                _logger.LogWarning(EventIds.Jobs, "Tried to update changelogs of guild {Guild}, but it was null", guildId);
                continue;
            }
            sentChangelogs++;
            
            // try to send new changelogs for each guild
            await PerformAsync(
                async ctx => await _updateGuildChangelogTask
                    .Execute(ctx, new UpdateGuildChangelogTask.Args(guild.Id))
                , new JobExecutionOptions {
                PlaceInQueue = guild.Id
                , RelatedGuildId = guild.Id
                , Name = "send new changelogs"
                , Debug = guild.Id.ToString()
            });
        }
        _logger.LogInformation(EventIds.Jobs, "Finished sending changelogs. {Sent} changelogs were sent", sentChangelogs);
    }
}