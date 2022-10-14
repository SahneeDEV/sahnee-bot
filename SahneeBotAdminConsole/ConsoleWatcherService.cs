using Microsoft.Extensions.Logging;

namespace SahneeBotAdminConsole;

public class ConsoleWatcherService
{
    private readonly ILogger<ConsoleWatcherService> _logger;
    private readonly ConsoleApplication _consoleApplication;
    private readonly Thread _watcherThread;
    
    public ConsoleWatcherService(ILogger<ConsoleWatcherService> logger,
        ConsoleApplication consoleApplication)
    {
        _logger = logger;
        _consoleApplication = consoleApplication;
        _watcherThread = new Thread(WatcherThread)
        {
            IsBackground = true,
            Priority = ThreadPriority.Lowest,
            Name = "ConsoleWatcherService"
        };
    }

    public void Install()
    {
        _logger.LogInformation("Installing console watcher");
        _watcherThread.Start();
    }

    private void WatcherThread()
    {
        _logger.LogInformation("Now watching for toggle to GUI mode");
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.F)
                {
                    _consoleApplication.Toggle();
                }
            }

            Thread.Sleep(100);
        }
    }
}