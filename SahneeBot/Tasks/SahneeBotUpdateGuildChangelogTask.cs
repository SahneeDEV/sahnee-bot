using SahneeBotController.Tasks.Changelog;

namespace SahneeBot.Tasks;

public class SahneeBotUpdateGuildChangelogTask : UpdateGuildChangelogTask
{
    private readonly Changelog _changelog;

    public SahneeBotUpdateGuildChangelogTask(IServiceProvider provider, Changelog changelog) : base(provider)
    {
        _changelog = changelog;
    }

    protected override Task<IEnumerable<Version>> GetVersionsAfter(Version version)
    {
        return Task.FromResult(_changelog.Versions
            .Where(v => v.Version > version)
            .Select(v => v.Version));
    }

    protected override Task<Version?> GetLatestVersion()
    {
        return Task.FromResult(_changelog.Versions.FirstOrDefault()?.Version);
    }
}