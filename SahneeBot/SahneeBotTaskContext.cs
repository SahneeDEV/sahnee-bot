using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot;

/// <summary>
/// Implementation of the task context for the sahnee bot.
/// </summary>
public class SahneeBotTaskContext: ITaskContext
{
    public SahneeBotTaskContext(string type, IServiceProvider provider, IServiceScope scope, SahneeBotModelContext model, 
        IDbContextTransaction? transaction)
    {
        Type = type;
        Provider = provider;
        Scope = scope;
        Model = model;
        Transaction = transaction;
    }

    /// <summary>
    /// The context type.
    /// </summary>
    public string Type { get; }
    public IServiceProvider Provider { get; }
    public IServiceScope Scope { get; }
    public SahneeBotModelContext Model { get; }
    public IDbContextTransaction? Transaction { get; }

    public void Dispose()
    {
    }
}