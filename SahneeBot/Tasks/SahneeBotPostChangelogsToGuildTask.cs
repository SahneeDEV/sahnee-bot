using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController;
using SahneeBotController.Tasks;
using SahneeBotController.Tasks.Changelog;

namespace SahneeBot.Tasks;

public class SahneeBotPostChangelogsToGuildTask : PostChangelogsToGuildTask
{
    private readonly Changelog _changelog;
    private readonly ChangelogVersionDiscordFormatter _fmt;
    private readonly Bot _bot;
    private readonly SahneeBotDiscordError _discordError;
    private readonly GetBoundChannelTask _boundChannelTask;
    private readonly ILogger<SahneeBotPostChangelogsToGuildTask> _logger;

    public SahneeBotPostChangelogsToGuildTask(Changelog changelog
        , ChangelogVersionDiscordFormatter fmt
        , GetBoundChannelTask boundChannelTask
        , Bot bot
        , SahneeBotDiscordError discordError
        , ILogger<SahneeBotPostChangelogsToGuildTask> logger)
    {
        _changelog = changelog;
        _fmt = fmt;
        _bot = bot;
        _discordError = discordError;
        _logger = logger;
        _boundChannelTask = boundChannelTask;
    }
    
    public override async Task<ISuccess<uint>> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, enumerable) = arg;
        _logger.LogDebug("Will now send changelogs to guild {Guild}", guildId);
        var set = new HashSet<Version>(enumerable);
        var changelogs = _changelog.Versions
            .Where(v => set.Contains(v.Version))
            .ToList();
        var boundChannel = await _boundChannelTask.Execute(ctx, new GetBoundChannelTask.Args(guildId));
        var guild = await _bot.Client.GetGuildAsync(guildId);
        if (guild == null)
        {
            _logger.LogWarning("Could not found the guild {GuildId}", guildId);
            return new Error<uint>("Could not find the server.");
        }

        var channel = boundChannel.HasValue
            ? await guild.GetTextChannelAsync(boundChannel.Value)
            : await guild.GetDefaultChannelAsync();
        if (channel == null)
        {
            _logger.LogWarning("Could not find a channel to post the changelogs in {GuildId}", guildId);
            return new Error<uint>("Could not find a channel to post the changelogs in.");
        }

        try
        {
            if (await _fmt.FormatAndSendMany(changelogs, channel.SendMessageAsync))
            {
                _logger.LogDebug("Sent the changelog to guild {Guild}", guildId);
                return new Success<uint>((uint) changelogs.Count);
            }
        }
        catch(Exception exception)
        {
            _logger.LogWarning(exception, "Failed to post changelogs in guild {Guild} due to error", guildId);
            var error = await _discordError.TryGetError<uint>(ctx
                                                              , new SahneeBotDiscordError.ErrorOptions {
                                                                  Exception = exception
                                                                  , GuildId = guildId
                                                                  , Hint = $"Could not post a changelog message in the channel {channel.Mention}."
                                                              });
            if (error != null)
            {
                return error;
            }

            throw;
        }

        _logger.LogDebug("No changelogs to post to {Guild}", guildId);
        return new Error<uint>("No changelogs to post.");
    }
}
