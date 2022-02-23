namespace SahneeBot.Events;

/// <summary>
/// Interface for all events.
/// </summary>
/// <typeparam name="TArg">The event argument type.</typeparam>
public interface IEvent<in TArg> : IEvent
{
    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="arg">The event argument.</param>
    /// <returns>Once the event has been handled.</returns>
    public Task Handle(TArg arg);
}

public interface IEvent
{
    /// <summary>
    /// Registers the handlers of this event.
    /// </summary>
    public void Register();
    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="arg">The event argument.</param>
    /// <returns>Once the event has been handled.</returns>
    public Task Handle(object arg);
}