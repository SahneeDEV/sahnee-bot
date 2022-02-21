namespace SahneeBot.Jobs;

public class JobTimeSpanRepeat : IJobTimeSpan
{
    //Variables
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
        //pretty output dirty code <3
        return $"{nameof(_interval)}: {(_interval.Days > 0 ? _interval.Days > 1 ? _interval.Days + "days " : _interval.Days + "day " : null)}" +
               $"{(_interval.Hours > 0 ? _interval.Hours > 1 ? _interval.Hours + "hours " : _interval.Hours + "hour " : null)}" +
               $"{(_interval.Minutes > 0 ? _interval.Minutes > 1 ? _interval.Minutes + "minutes " : _interval.Minutes + "minute " : null)}" +
               $"{(_interval.Seconds > 0 ? _interval.Seconds > 1 ? _interval.Seconds + "seconds " : _interval.Seconds + "second " : null)}" +
               $"{(_interval.Milliseconds > 0 ? _interval.Milliseconds > 1 ? _interval.Milliseconds + "milliseconds " : _interval.Milliseconds + "millisecond " : null)}";
    }
}
