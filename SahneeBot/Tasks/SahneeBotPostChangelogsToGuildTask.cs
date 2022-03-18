﻿using SahneeBot.Formatter;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotPostChangelogsToGuildTask : PostChangelogsToGuildTask
{
    private readonly Changelog _changelog;
    private readonly ChangelogVersionDiscordFormatter _fmt;
    private readonly Bot _bot;
    private readonly GetBoundChannelTask _boundChannelTask;

    public SahneeBotPostChangelogsToGuildTask(Changelog changelog
        , ChangelogVersionDiscordFormatter fmt
        , GetBoundChannelTask boundChannelTask
        , Bot bot)
    {
        _changelog = changelog;
        _fmt = fmt;
        _bot = bot;
        _boundChannelTask = boundChannelTask;
    }
    
    public override async Task<ISuccess<uint>> Execute(ITaskContext ctx, Args arg)
    {
        var (guildId, enumerable) = arg;
        var set = new HashSet<Version>(enumerable);
        var changelogs = _changelog.Versions
            .Where(v => set.Contains(v.Version))
            .ToList();
        var boundChannel = await _boundChannelTask.Execute(ctx, new GetBoundChannelTask.Args(guildId));
        var guild = await _bot.Client.GetGuildAsync(guildId);
        if (guild == null)
        {
            return new Error<uint>("Could not find the server.");
        }

        var channel = boundChannel.HasValue
            ? await guild.GetTextChannelAsync(boundChannel.Value)
            : await guild.GetDefaultChannelAsync();
        if (channel == null)
        {
            return new Error<uint>("Could not find a channel to post the changelogs in.");
        }

        if (await _fmt.FormatAndSendMany(changelogs, channel.SendMessageAsync))
        {
            return new Success<uint>((uint) changelogs.Count);
        }

        return new Error<uint>("No changelogs to post.");
    }
}