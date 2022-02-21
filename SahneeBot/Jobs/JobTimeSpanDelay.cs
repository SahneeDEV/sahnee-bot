namespace SahneeBot.Jobs;

public class JobTimeSpanDelay : IJobTimeSpan
{
    //Variables
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
        return $"{nameof(_delay)}: {_delay}, {nameof(_fresh)}: {_fresh}";
    }
}