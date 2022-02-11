using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBotModel;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gives the given user a warning.
/// </summary>
public class GiveWarningToUserTask
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<GiveWarningToUserTask> _logger;
    private readonly IdGenerator _id;

    public GiveWarningToUserTask(IServiceProvider provider, ILogger<GiveWarningToUserTask> logger, IdGenerator id)
    {
        _provider = provider;
        _logger = logger;
        _id = id;
    }

    public async Task Execute(ulong guildId, ulong issuerUserId, ulong userId, string reason)
    {
        using var scope = _provider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<SahneeBotModelContext>();
        _logger.LogInformation("Execute warn task!");
        var warn = new Warning
        {
            Id = _id.NextId(),
            Number = 1,
            Reason = reason,
            UserId = userId,
            GuildId = guildId,
            IssuerUserId = issuerUserId
        };
        await ctx.Warnings.AddAsync(warn);
        await ctx.SaveChangesAsync();
    }
}
