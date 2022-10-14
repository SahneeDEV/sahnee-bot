using Microsoft.Extensions.Logging;
using Terminal.Gui;

namespace SahneeBotAdminConsole;

public class ConsoleApplication
{
    private readonly ILogger<ConsoleApplication> _logger;
    private readonly ConsoleLoggingController _loggingController;
    private bool _active;

    public ConsoleApplication(ILogger<ConsoleApplication> logger, ConsoleLoggingController loggingController)
    {
        _logger = logger;
        _loggingController = loggingController;
    }

    public void Toggle()
    {
        _active = !_active;
        _loggingController.Enabled = !_active;
        _logger.LogInformation("Console application is now active: {0}", _active);
        if (_active)
        {
            Console.Clear();
            Application.Init();
new ConsoleOverviewGui().Setup();
        }
        else
        {
            Application.Shutdown();
        }
    }
}