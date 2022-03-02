using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Tasks;
using SahneeBotController.Tasks;

namespace SahneeBot.InteractionComponents;

public class SelectMenuExecution
{
    private readonly ILogger<SelectMenuExecution> _logger;
    private readonly SahneeBotRemoveUserFromGuildState _sahneeBotRemoveUserFromGuildState;

    public SelectMenuExecution(ILogger<SelectMenuExecution> logger
    , SahneeBotRemoveUserFromGuildState sahneeBotRemoveUserFromGuildState)
    {
        _logger = logger;
        _sahneeBotRemoveUserFromGuildState = sahneeBotRemoveUserFromGuildState;
    }

    /// <summary>
    /// Execution of the Select Menu interactions
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="arg"></param>
    /// <exception cref="Exception"></exception>
    public async Task Execute(ITaskContext ctx, SocketMessageComponent arg)
    {
        await arg.DeferLoadingAsync(true);
        //Switch between all the custom id's of the select menus
        switch (arg.Data.CustomId)
        {
            case "remove-guild-users-from-db":
                await _sahneeBotRemoveUserFromGuildState.Execute(ctx, new SahneeBotRemoveUserFromGuildState.Args(arg));
                break;
            default:
                _logger.LogCritical(EventIds.Discord, "Could not find a case for the given SelectMenu");
                throw new Exception("No SelectMenu found with the given id");
        }
    }
}