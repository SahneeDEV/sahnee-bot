using System;
using System.Collections.Generic;
using sahnee_bot.Logging;

namespace sahnee_bot.Queue
{
    public class QueueManager
    {
        //Variables
        private Dictionary<ulong,MultiThreadQueue> _availableQueues = new Dictionary<ulong,MultiThreadQueue>();
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Creates a new FiFo queue for a guild
        /// </summary>
        /// <param name="guildId">the id of the guild to identify the queue</param>
        public MultiThreadQueue CreateNewQueueForGuild(ulong guildId)
        {
            MultiThreadQueue newQueue = new MultiThreadQueue(guildId);
            _availableQueues.Add(guildId, newQueue);
            _logger.Log($"Successfully created a new queue for Guild: {guildId}", LogLevel.Info, "QueueManager:CreateNewQueueForGuild");
            return newQueue;
        }

        /// <summary>
        /// Checks if a FiFo queue already exists for a guild otherwise will create a new one
        /// </summary>
        /// <param name="guildId"></param>
        public MultiThreadQueue CheckIfQueueForGuildExistsOrCreate(ulong guildId)
        {
            try
            {
                //queue already exists
                if (_availableQueues.ContainsKey(guildId))
                {
                    return _availableQueues[guildId];
                }
                //create and return a new queue
                return CreateNewQueueForGuild(guildId);
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogLevel.Error, "QueueManager:CheckIfQueueForGuildExistsOrCreate");
                return null;
            }
        }

        /// <summary>
        /// Removes a queue and destroys everything enqueued
        /// </summary>
        /// <param name="guildId"></param>
        public void DeleteGuildQueue(ulong guildId)
        {
            try
            {
                //check if queue exists for guild
                if (!_availableQueues.ContainsKey(guildId))
                {
                    _logger.Log($"Someone wanted to delete a queue that did not exist this guild: {guildId}",
                        LogLevel.Info, "QueueManager:DeleteGuildQueue");
                    return;
                }
                _availableQueues[guildId].DestroyQueue();
                _availableQueues.Remove(guildId);
                _logger.Log($"Successfully destroyed the queue for guild: {guildId}", LogLevel.Info, "QueueManager:DeleteGuildQueue");
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogLevel.Error, "QueueManager:DeleteGuildQueue");
            }
        }

        /// <summary>
        /// Clears all objects from a queue
        /// </summary>
        /// <param name="guildId"></param>
        public void ClearQueue(ulong guildId)
        {
            try
            {
                //get the guilds queue
                MultiThreadQueue currentQueue = _availableQueues[guildId];
                //clear the queue
                currentQueue.ClearQueue();
                _logger.Log($"Successfully cleared the queue for guild: {guildId}", LogLevel.Info, "QueueManager:ClearQueue");
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogLevel.Error, "QueueManager:ClearQueue");
            }
        }
    }
}
