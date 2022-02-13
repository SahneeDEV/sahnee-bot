using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot;

public class SahneeBotTaskContext: ITaskContext
{
    /*public SahneeBotTaskContext(IServiceProvider provider)
    { 
        Provider = provider;
        Scope = provider.CreateScope();
        Model = Scope.ServiceProvider.GetRequiredService<SahneeBotModelContext>();
    }*/

    public SahneeBotTaskContext(IServiceProvider provider, IServiceScope scope, SahneeBotModelContext model, 
        IDbContextTransaction? transaction)
    {
        Provider = provider;
        Scope = scope;
        Model = model;
        Transaction = transaction;
    }

    public IServiceProvider Provider { get; }
    public IServiceScope Scope { get; }
    public SahneeBotModelContext Model { get; }
    public IDbContextTransaction? Transaction { get; }

    public void Dispose()
    {
    }
}