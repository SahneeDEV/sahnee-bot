using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using sahnee_bot.Util;

namespace sahnee_bot.Jobs
{
    public class JobHandler
    {
        //Variables
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Thread _jobThread;
        private readonly Dictionary<Guid,JobDetails> _jobs = new Dictionary<Guid, JobDetails>();
        private readonly Logging _logging = new Logging();

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
        public Guid RegisterJob(IJobTimeSpan jobTimeSpan, Func<Task> action)
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
                _logging.LogToConsoleBase($"Started Job: {id}, TimeSpan: {jobTimeSpan}");
            }

            return id;
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
                foreach (var job in jobsToExecute)
                {
                    Task.Run(job.Action).ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            _logging.LogToConsoleBase($"Job: {job.Guid} failed! \n {task.Exception}");
                        }
                    });
                }
                
                //clear the jobsToExecute list
                jobsToExecute.Clear();
                
                Thread.Sleep(100);
            }
        }
        
    }
}
