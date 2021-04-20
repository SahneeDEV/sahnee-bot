using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.Jobs
{
    public class JobHandler
    {
        //Variables
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Thread _jobThread;
        private readonly Dictionary<Guid,JobDetails> _jobs = new Dictionary<Guid, JobDetails>();
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// JobDetails
        /// </summary>
        private class JobDetails
        {
            public IJobTimeSpan JobTimeSpan;
            public Func<Task> Action;
            public DateTime NextRunTime;
            public Guid Guid;
        }

        
        public JobHandler()
        {
            _jobThread = new Thread(JobThread);
            _jobThread.IsBackground = true;
            _jobThread.Priority = ThreadPriority.AboveNormal;
            _jobThread.Start();
        }

        /// <summary>
        /// Registers a new job
        /// </summary>
        /// <param name="jobTimeSpan">the timespan for the job</param>
        /// <param name="action">action that will be executed</param>
        /// <returns>unique Guid</returns>
        public Guid? RegisterJob(IJobTimeSpan jobTimeSpan, Func<Task> action)
        {
            try
            {
                Guid id = Guid.NewGuid();
                lock (_jobs)
                {
                    var nextRunTime = jobTimeSpan.GetNextExecutionTime();
                    if (!nextRunTime.HasValue)
                    {
                        throw new InvalidOperationException("Cannot start job without valid starttime");
                    }
                    _jobs.Add(id,new JobDetails() { JobTimeSpan = jobTimeSpan, Action = action, NextRunTime = nextRunTime.Value, Guid = id});
                    _logger.Log($"Started Job: {id}, TimeSpan: {jobTimeSpan}", LogLevel.Info);
                }

                return id;
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogLevel.Error,"CleanupWarningRolesAction:CleanupWarningRolesAsync");
                return null;
            }
        }

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
                                _logger.Log($"Job: {job.Guid} failed! \n {task.Exception}", LogLevel.Error);
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    _logger.Log(e.Message, LogLevel.Error, "CleanupWarningRolesAction:CleanupWarningRolesAsync");
                }

                //clear the jobsToExecute list
                jobsToExecute.Clear();
                
                Thread.Sleep(100);
            }
        }
    }
}
