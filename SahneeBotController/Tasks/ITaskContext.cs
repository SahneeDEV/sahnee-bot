using Microsoft.Extensions.DependencyInjection;
using SahneeBotModel;

namespace SahneeBotController.Tasks;

public interface ITaskContext: IDisposable
{
    IServiceProvider Provider { get; }
    IServiceScope Scope { get; }
    SahneeBotModelContext Model { get; }
}
