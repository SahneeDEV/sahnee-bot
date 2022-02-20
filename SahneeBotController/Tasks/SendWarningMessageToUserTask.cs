using Microsoft.Extensions.DependencyInjection;
using SahneeBotModel.Contract;

namespace SahneeBotController.Tasks;

/// <summary>
/// Send a user a warning message.
/// </summary>
public abstract class SendWarningMessageToUserTask: ITask<SendWarningMessageToUserTask.Args, bool>
{
    private readonly GetMessageOptOutTask _optOutTask;
    private readonly SendMessageOptOutHintToUserTask _sendMessageOptOutHintToUserTask;

    /// <summary>
    /// Arguments for sending the message.
    /// </summary>
    public record struct Args(IWarning Warning, ulong RecipientId);

    protected SendWarningMessageToUserTask(IServiceProvider provider)
    {
        _optOutTask = provider.GetRequiredService<GetMessageOptOutTask>();
        _sendMessageOptOutHintToUserTask = provider.GetRequiredService<SendMessageOptOutHintToUserTask>();
    }

    public async Task<bool> Execute(ITaskContext ctx, Args arg)
    {
        var (warning, recipientId) = arg;
        // Don't send a message if the user opted out.
        var optedOut = await _optOutTask.Execute(ctx, new GetMessageOptOutTask.Args(warning.UserId, 
            warning.GuildId));
        if (optedOut)
        {
            return false;
        }

        var res = await ExecuteImpl(ctx, arg);
        // If the message was sent also send an opt out hint
        if (res)
        {
            await _sendMessageOptOutHintToUserTask.Execute(ctx, new SendMessageOptOutHintToUserTask.Args(
                recipientId, warning.GuildId));
        }

        return res;
    }

    protected abstract Task<bool> ExecuteImpl(ITaskContext ctx, Args arg);
}