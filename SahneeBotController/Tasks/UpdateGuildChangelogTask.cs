using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SahneeBotController.Tasks;

/// <summary>
/// Updates the changelog version of a guild to the latest and returns the new version of the guild.
/// </summary>
public abstract class UpdateGuildChangelogTask : ITask<UpdateGuildChangelogTask.Args, ISuccess<Version?>>
{
    private readonly GetGuildStateTask _getGuildStateTask;
    private readonly PostChangelogsToGuildTask _postChangelogsToGuildTask;
    private readonly ILogger<UpdateGuildChangelogTask> _logger;

    /// <summary>
    /// Arguments for this task.
    /// </summary>
    /// <param name="GuildId">The guild ID to update the changelog for.</param>
    public record struct Args(ulong GuildId);
    
    protected UpdateGuildChangelogTask(IServiceProvider provider)
    {
        _getGuildStateTask = provider.GetRequiredService<GetGuildStateTask>();
        _postChangelogsToGuildTask = provider.GetRequiredService<PostChangelogsToGuildTask>();
        _logger = provider.GetRequiredService<ILogger<UpdateGuildChangelogTask>>();
    }

    public async Task<ISuccess<Version?>> Execute(ITaskContext ctx, Args arg)
    {
        var state = await _getGuildStateTask.Execute(ctx, new GetGuildStateTask.Args(arg.GuildId));
        var lastVersion = state.LastChangelogVersion;
        var latestVersion = await GetLatestVersion();
        ISuccess<Version?>? success = null;
        if (lastVersion == latestVersion)
        {
            _logger.LogDebug(EventIds.Changelog, "Guild {Guild} already got the latest" +
                                                 " changelog: {Version}", arg.GuildId, latestVersion);
            return new Success<Version?>(lastVersion);
        }
        // Don't post a changelog for new guilds and also don't post old versions we skipped
        if (lastVersion != null && lastVersion < latestVersion)
        {
            var newVersions = (await GetVersionsAfter(lastVersion)).ToArray();
            _logger.LogDebug(EventIds.Changelog
                , "Posting {Count} changelogs from {OldVersion} to {NewVersion} to guild {Guild}"
                , newVersions.Length, lastVersion, latestVersion, arg.GuildId);
            if (newVersions.Length > 0)
            {
                var post = await _postChangelogsToGuildTask.Execute(ctx, new PostChangelogsToGuildTask.Args(arg.GuildId, newVersions));
                if (!post.IsSuccess)
                {
                    _logger.LogWarning(EventIds.Changelog
                        , "Failed to post changelog to guild {Guild}: {Error}"
                        , arg.GuildId, post.Message);
                    success = new Error<Version?>(post.Message);
                }
            }
        }

        _logger.LogInformation(EventIds.Changelog
            , "Changing changelog version from {OldVersion} to {NewVersion} in guild {Guild}"
            , lastVersion, latestVersion, arg.GuildId);
        state.LastChangelogVersion = latestVersion;
        await ctx.Model.SaveChangesAsync();
        return success ?? new Success<Version?>(latestVersion);
    }

    /// <summary>
    /// Gets all versions in the changelog after the given version.
    /// </summary>
    /// <param name="version">The version to get versions after.</param>
    /// <returns>All versions after the given version</returns>
    protected abstract Task<IEnumerable<Version>> GetVersionsAfter(Version version);
    /// <summary>
    /// Gets the latest available version.
    /// </summary>
    /// <returns>The latest version, or null if none.</returns>
    protected abstract Task<Version?> GetLatestVersion();
}