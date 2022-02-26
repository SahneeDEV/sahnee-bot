using Microsoft.Extensions.Logging;

namespace SahneeBot.Jobs;

/// <summary>
/// The job handler contains logic to execute jobs in the given interval/after a delay, etc...
/// </summary>
public class JobHandler
{
    private readonly ILogger<JobHandler> _logger;
    private readonly Dictionary<Guid,JobDetails> _jobs = new();
    private bool _running = true;
    
    /// <summary>
    /// Contains details about a single job.
    /// </summary>
    private class JobDetails
    {
        /// <summary>
        /// When will the job be executed next?
        /// </summary>
        public readonly IJobTimeSpan JobTimeSpan;
        /// <summary>
        /// The actual job action.
        /// </summary>
        public readonly Func<Task> Action;
        /// <summary>
        /// The ID of the job.
        /// </summary>
        public readonly Guid Guid;
        /// <summary>
        /// When is the next executions scheduled?
        /// </summary>
        public DateTime NextRunTime;

        public JobDetails(Guid guid, Func<Task> action, IJobTimeSpan jobTimeSpan)
        {
            JobTimeSpan = jobTimeSpan;
            Action = action;
            Guid = guid;
        }
    }
    
    public JobHandler(ILogger<JobHandler> logger)
    {
        _logger = logger;
        var jobThread = new Thread(JobThread)
        {
            IsBackground = true,
            Priority = ThreadPriority.AboveNormal
        };
        jobThread.Start();
    }

    /// <summary>
    /// Arguments for Registering a job
    /// </summary>
    /// <param name="JobTimeSpan">the time span in which the job will be repeatedly executed</param>
    /// <param name="Action">the action that will be called on execution</param>
    public record struct Args(IJobTimeSpan JobTimeSpan, Func<Task> Action);

    /// <summary>
    /// Registers a new job
    /// </summary>
    /// <param name="args">the args</param>
    /// <returns>unique Guid</returns>
    public Guid? RegisterJob(Args args)
    {
        var (jobTimeSpan, action) = args;
        try
        {
            var guid = Guid.NewGuid();
            lock (_jobs)
            {
                var nextRunTime = jobTimeSpan.GetNextExecutionTime();
                if (!nextRunTime.HasValue)
                {
                    throw new InvalidOperationException("Cannot start job without valid start-time");
                }
                _jobs.Add(guid, new JobDetails(guid, action, jobTimeSpan)
                {
                    NextRunTime = nextRunTime.Value
                });
                _logger.LogDebug(EventIds.Jobs, "Started Job: {Guid} TimeSpan: {JobTimeSpan}",
                    guid, jobTimeSpan);
            }

            return guid;
        }
        catch (Exception e)
        {
            _logger.LogCritical(EventIds.Jobs, e, "Creating a Job failed");
            return null;
        }
    }
    
    /// <summary>
    /// The Thread that will execute the Jobs
    /// </summary>
    private void JobThread()
    {
        //Variables
        var jobsToExecute = new List<JobDetails>();
            
        //Keep alive
        while (_running)
        {
            //Get the current time
            var now = DateTime.Now;

            lock (_jobs)
            {
                //Append all jobs to the list
                foreach (var (guid, job) in _jobs)
                {
                    if (job.NextRunTime <= now)
                    {
                        jobsToExecute.Add(job);
                    }
                }

                //Check if job needs to be executed again or can be removed from the list
                foreach (var jobToExecute in jobsToExecute)
                {
                    var nextRunTime = jobToExecute.JobTimeSpan.GetNextExecutionTime();
                    if (!nextRunTime.HasValue)
                    {
                        _jobs.Remove(jobToExecute.Guid);
                    }
                    else
                    {
                        jobToExecute.NextRunTime = nextRunTime.Value;
                    }
                }
            }
            //Execute jobs
            foreach (var job in jobsToExecute)
            {
                Task.Run(job.Action).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.LogCritical(EventIds.Jobs, task.Exception,
                            "Job {JobId} failed!", job.Guid);
                    }
                });
            }
            //clear the jobsToExecute list
            jobsToExecute.Clear();
            Thread.Sleep(50);
        }
    }
}
