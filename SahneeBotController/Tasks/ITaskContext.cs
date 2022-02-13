using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using SahneeBotModel;

namespace SahneeBotController.Tasks;

/// <summary>
/// Provides a context to a single task.
/// </summary>
public interface ITaskContext: IDisposable
{
    /// <summary>
    /// The global service provider.
    /// </summary>
    IServiceProvider Provider { get; }
    /// <summary>
    /// The scope of the task.
    /// </summary>
    IServiceScope Scope { get; }
    /// <summary>
    /// The db context of the task.
    /// </summary>
    SahneeBotModelContext Model { get; }
    /// <summary>
    /// The database transaction.
    /// </summary>
    IDbContextTransaction? Transaction { get; }
}
