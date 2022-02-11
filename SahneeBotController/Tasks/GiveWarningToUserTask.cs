using SahneeBotModel;
using SahneeBotModel.Models;

namespace SahneeBotController.Tasks;

/// <summary>
/// Gives the given user a warning.
/// </summary>
public class GiveWarningToUserTask
{
    private readonly SahneeBotModelContext _sahneeBotModelContext;
    
    public GiveWarningToUserTask(SahneeBotModelContext sahneeBotModelContext)
    {
        _sahneeBotModelContext = sahneeBotModelContext;
    }

    public void Execute(ulong userId)
    {
        var warn = new Warning
        {
            Reason = "bla",
            UserId = userId
        };
        _sahneeBotModelContext.Warnings.Add(warn);
    }
}
