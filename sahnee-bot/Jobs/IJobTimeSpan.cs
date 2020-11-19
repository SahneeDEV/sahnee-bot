using System;

namespace sahnee_bot.Jobs
{
    public interface IJobTimeSpan
    {

        /// <summary>
        /// Returns the next execution-time of the job
        /// </summary>
        /// <returns></returns>
        public DateTime? GetNextExecutionTime();
    }
}
