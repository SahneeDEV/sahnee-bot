using Discord.Interactions;
using SahneeBot.Formatter;
using SahneeBotController.Tasks.Changelog;

namespace SahneeBot.Commands;

/// <summary>
/// Allows the user to print the bot changelog.
/// </summary>
public class ChangelogCommand : CommandBase
{
    private readonly GetLastChangelogOfGuildTask _getLastChangelogOfGuildTask;
    private readonly Changelog _changelog;
    private readonly ChangelogVersionDiscordFormatter _fmt;
    private readonly NoChangelogFoundDiscordFormatter _noFmt;

    public ChangelogCommand(
        IServiceProvider serviceProvider,
        GetLastChangelogOfGuildTask getLastChangelogOfGuildTask,
        Changelog changelog,
        ChangelogVersionDiscordFormatter fmt,
        NoChangelogFoundDiscordFormatter noFmt
        ) : base(serviceProvider)
    {
        _getLastChangelogOfGuildTask = getLastChangelogOfGuildTask;
        _changelog = changelog;
        _fmt = fmt;
        _noFmt = noFmt;
    }

    [SlashCommand("changelog", "Gets the changelog of the bot")]
    public Task GetCommand(
        [Summary("version", "The version to get the changelog of (use 'initial' to get the first changelog)")]
        string? versionRaw = null,
        [Summary(description: "Gets all changelogs after the given version")]
        bool all = false
        ) => ExecuteAsync(async ctx =>
    {
        Version? startVersion;
        if (versionRaw == null && all)
        {
            versionRaw = "initial";
        }
        if (string.Equals(versionRaw, "initial", StringComparison.OrdinalIgnoreCase))
        {
            startVersion = _changelog.Versions.LastOrDefault()?.Version;
        } 
        else if (Version.TryParse(versionRaw, out var version))
        {
            startVersion = version;
        }
        else
        {
            startVersion = await _getLastChangelogOfGuildTask.Execute(ctx, 
                new GetLastChangelogOfGuildTask.Args(Context.Guild.Id));
        }

        var versions = startVersion == null
            ? Array.Empty<Changelog.VersionInformation>()
            : all
            ? _changelog.Versions.SkipWhile(v => v.Version < startVersion)
            : _changelog.Versions.Where(v => v.Version == startVersion);
        if (!await _fmt.FormatAndSendMany(versions, ModifyOriginalResponseAsync, SendChannelMessageAsync))
        {
            await _noFmt.FormatAndSend(new NoChangelogFoundDiscordFormatter.Args(Context.Guild.Id, startVersion, all),
                ModifyOriginalResponseAsync);
        }
    });
}