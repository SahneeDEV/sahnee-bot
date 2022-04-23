namespace SahneeBotController;

/// <summary>
/// A command
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <returns>Once the command has been executed.</returns>
    Task Do();
    /// <summary>
    /// Undoes the command.
    /// </summary>
    /// <returns>Once the command has been undone.</returns>
    Task Undo();
}

/// <summary>
/// Basic command implementation.
/// </summary>
public class Command : ICommand
{
    private readonly Fn _doFn;
    private readonly Fn _undoFn;

    public delegate Task Fn();
    
    private Command(Fn doFn, Fn undoFn)
    {
        _doFn = doFn;
        _undoFn = undoFn;
    }

    public static ICommand CreateSimple(Fn doFn, Fn undoFn)
    {
        return new Command(doFn, undoFn);
    }

    public Task Do()
    {
        return _doFn();
    }

    public Task Undo()
    {
        return _undoFn();
    }

    /// <summary>
    /// Executes all commands. If any fails, undoes all.
    /// </summary>
    /// <param name="commands">The commands to execute.</param>
    public static async Task DoAll(IEnumerable<ICommand> commands)
    {
        var success = new List<ICommand>();
        IList<Exception>? fail = null;
        foreach (var command in commands)
        {
            try
            {
                await command.Do();
                success.Add(command);
            }
            catch (Exception exception)
            {
                fail = new List<Exception> { exception };
                break;
            }
        }

        if (fail != null)
        {
            foreach (var successCommand in success)
            {
                try
                {
                    await successCommand.Undo();
                }
                catch (Exception e)
                {
                    fail.Add(e);
                }
            }

            if (fail.Count == 1)
            {
                throw fail[0];
            }

            throw new AggregateException(fail);
        }
    }
}
