namespace SahneeBot.Jobs;

/// <summary>
/// Interface for implementing jobs.
/// </summary>
public interface IJob
{
    /// <summary>
    /// Executes the job.
    /// </summary>
    /// <returns>Once the job is done.</returns>
    public Task Perform();
    /// <summary>
    /// The time span of the job.
    /// </summary>
    public IJobTimeSpan Time { get; }
}