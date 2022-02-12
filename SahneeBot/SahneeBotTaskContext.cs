using Microsoft.Extensions.DependencyInjection;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot;

public class SahneeBotTaskContext: ITaskContext
{
    public SahneeBotTaskContext(IServiceProvider provider)
    { 
        Provider = provider;
        Scope = provider.CreateScope();
        Model = Scope.ServiceProvider.GetRequiredService<SahneeBotModelContext>();
    }

    public IServiceProvider Provider { get; }
    public IServiceScope Scope { get; }
    public SahneeBotModelContext Model { get; }

    public void Dispose()
    {
        Scope.Dispose();
        Model.Dispose();
    }
}