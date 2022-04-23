using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SahneeBot.Events;

public class EventHandler : IEventHandler
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<EventHandler> _logger;
    private IEvent[] _events = Array.Empty<IEvent>();

    public EventHandler(IServiceProvider provider)
    {
        _provider = provider;
        _logger = provider.GetRequiredService<ILogger<EventHandler>>();
    }

    public void Install()
    {
        AddModulesAsync(Assembly.GetEntryAssembly()!, _provider);
    }

    private Task AddModulesAsync(Assembly assembly, IServiceProvider provider)
    {
        //provider.GetService()
        _events = assembly
            .GetTypes()
            .Where(type => type.GetCustomAttribute<EventAttribute>() != null)
            .Select(type => ReflectionUtils<IEvent>.CreateBuilder<IEventHandler>(type.GetTypeInfo(), this))
            .Select(func => func(provider))
            .ToArray();
        foreach (var evt in _events)
        {
            _logger.LogDebug("Registering event {Event}", evt);
            evt.Register();
        }
        return Task.CompletedTask;
    }
}