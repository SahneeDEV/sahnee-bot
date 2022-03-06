namespace SahneeBot.Jobs;

/// <summary>
/// A time spawn that keeps repeating forever with the given interval between executions.
/// </summary>
public class JobTimeSpanRepeat : IJobTimeSpan
{
    private readonly TimeSpan _interval;

    public JobTimeSpanRepeat(TimeSpan interval)
    {
        _interval = interval;
    }
    
    public DateTime? GetNextExecutionTime()
    {
        return DateTime.Now + _interval;
    }

    public override string ToString()
    {
        return $"JobTimeSpanRepeat(Interval: {_interval})";
    }
}
