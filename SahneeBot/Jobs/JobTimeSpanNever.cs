namespace SahneeBot.Jobs;

/// <summary>
/// A job time span that never executes.
/// </summary>
public class JobTimeSpanNever : IJobTimeSpan
{
    /// <summary>
    /// The time span.
    /// </summary>
    public static IJobTimeSpan Instance { get; } = new JobTimeSpanNever();
    
    private JobTimeSpanNever() {}
    
    public DateTime? GetNextExecutionTime()
    {
        return null;
    }

    public override string ToString()
    {
        return "JobTimeSpanNever()";
    }
}