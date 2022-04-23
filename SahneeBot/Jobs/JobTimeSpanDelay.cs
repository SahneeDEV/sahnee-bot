namespace SahneeBot.Jobs;

/// <summary>
/// A time span that executes once after the given delay.
/// </summary>
public class JobTimeSpanDelay : IJobTimeSpan
{
    private readonly TimeSpan _delay;
    private bool _fresh;

    public JobTimeSpanDelay(TimeSpan delay)
    {
        _delay = delay;
    }
        
    public DateTime? GetNextExecutionTime()
    {
        if (_fresh)
        {
            _fresh = false;
            return DateTime.Now + _delay;
        }
        return null;
    }

    public override string ToString()
    {
        return $"JobTimeSpanDelay(Delay: {_delay}, Fresh: {_fresh})";
    }
}