using Discord;
using Microsoft.Extensions.Logging;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

/// <summary>
/// Updates the bot activity.
/// </summary>
public class SahneeBotActivityTask : ITask<SahneeBotActivityTask.Args, ISuccess<IActivity>>
{
    private class ActivityWatchingGuildsAmount : IActivity
    {
        public string Name { get; }
        public ActivityType Type { get; }
        public ActivityProperties Flags { get; }
        public string? Details => null;
    
        public ActivityWatchingGuildsAmount(int amount)
        {
            Name = $"on {amount} server{(amount == 1 ? "" : "s")}";
            Type = ActivityType.Watching;
            Flags = ActivityProperties.None;
        }
    }
    
    public record struct Args;
    
    private readonly ILogger<SahneeBotActivityTask> _logger;
    private readonly Bot _bot;

    public SahneeBotActivityTask(ILogger<SahneeBotActivityTask> logger
        , Bot bot)
    {
        _logger = logger;
        _bot = bot;
    }

    public Task<ISuccess<IActivity>> Execute(ITaskContext ctx, Args arg)
    {
        return _bot.ImplAsync(async socket =>
        {
            try
            {
                var activity = new ActivityWatchingGuildsAmount(socket.Guilds.Count);
                await socket.SetActivityAsync(activity);
                return new Success<IActivity>(activity);
            }
            catch (Exception e)
            {
                _logger.LogCritical(EventIds.Discord, e, "Could not update the bot activity!");
                return new Error<IActivity>(e.Message);
            }
        }, rest => Task.FromResult(
            new Error<IActivity>("Bot activity is not supported in the Rest implementation.") as ISuccess<IActivity>));
    }
}
