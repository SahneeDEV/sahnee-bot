namespace SahneeBot.Jobs;

/// <summary>
/// Interface for implementing the repetition of a job.
/// </summary>
public interface IJobTimeSpan
{
    /// <summary>
    /// Returns the next execution-time of the job.
    /// </summary>
    /// <remarks>
    /// Calling this is <b>not</b> a pure operation. The class assumes that the returned date has been consumed and will
    /// be handled. Cache it if required.
    /// </remarks>
    /// <returns>The next time, or null if none.</returns>
    public DateTime? GetNextExecutionTime();
}