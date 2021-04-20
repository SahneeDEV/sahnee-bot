namespace sahnee_bot.Queue
{
    /// <summary>
    /// Provides the same QueueManager to everyone
    /// </summary>
    public static class QueueFactory
    {
        //Variables
        private static readonly QueueManager QueueManager = new QueueManager();

        /// <summary>
        /// Returns the queuemanager
        /// </summary>
        /// <returns></returns>
        public static QueueManager GetQueueManager()
        {
            return QueueManager;
        }
    }
}
