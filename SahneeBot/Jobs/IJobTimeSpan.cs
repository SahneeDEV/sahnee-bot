namespace SahneeBot.Jobs;

public interface IJobTimeSpan
{

    /// <summary>
    /// Returns the next execution-time of the job
    /// </summary>
    /// <returns></returns>
    public DateTime? GetNextExecutionTime();
}