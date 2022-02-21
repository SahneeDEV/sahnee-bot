using Microsoft.Extensions.Logging;

namespace SahneeBot.Jobs;

public class JobHandler
{
    private readonly ILogger<JobHandler> _logger;
    private readonly Dictionary<Guid,JobDetails> _jobs = new();
    
    /// <summary>
    /// JobDetails
    /// </summary>
    private class JobDetails
    {
        public IJobTimeSpan JobTimeSpan = null!;
        public Func<Task> Action = null!;
        public DateTime NextRunTime;
        public Guid Guid;
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
    /// <param name="GuildId">the guildId the job is for</param>
    public record struct Args(IJobTimeSpan JobTimeSpan, Func<Task> Action);

    /// <summary>
    /// Registers a new job
    /// </summary>
    /// <param name="args">the args</param>
    /// <returns>unique Guid</returns>
    public Guid? RegisterJob(Args args)
    {
        try
        {
            Guid guid = Guid.NewGuid();
            lock (_jobs)
            {
                var nextRunTime = args.JobTimeSpan.GetNextExecutionTime();
                if (!nextRunTime.HasValue)
                {
                    throw new InvalidOperationException("Cannot start job without valid start-time");
                }
                _jobs.Add(guid,new JobDetails { JobTimeSpan = args.JobTimeSpan, Action = args.Action
                    , NextRunTime = nextRunTime.Value, Guid = guid});
                _logger.LogDebug(EventIds.Jobs, $"Started Job: {guid}" +
                                                $", TimeSpan: {args.JobTimeSpan}");
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
        List<JobDetails> jobsToExecute = new List<JobDetails>();
            
        //Keep alive
        while (true)
        {
            //Get the current time
            var now = DateTime.Now;

            lock (_jobs)
            {
                    //Append all jobs to the list
                foreach (var job in _jobs)
                {
                    if (job.Value.NextRunTime <= now)
                    {
                            jobsToExecute.Add(job.Value);
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
            try
            {
                foreach (var job in jobsToExecute)
                {
                    Task.Run(job.Action).ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            _logger.LogCritical(EventIds.Jobs
                                , "Job {jobId} failed! \n {exception}", job.Guid, task.Exception);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(EventIds.Jobs, e, "Job crashed!");
            }
            //clear the jobsToExecute list
            jobsToExecute.Clear();
            Thread.Sleep(50);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}
