using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Timers;
using sahnee_bot.Logging;

namespace sahnee_bot.Queue
{
    public class MultiThreadQueue
    {
        //Variables
        private ConcurrentQueue<IQueueMessage> _messageQueue = new ConcurrentQueue<IQueueMessage>();
        private bool _destroyQueue = false;
        private readonly Logger _logger = new Logger();
        private ulong _guildId = 0;

        /// <summary>
        /// Creates a new Multithreaded FiFo Queue for the commands
        /// </summary>
        public MultiThreadQueue(ulong guildId)
        {
            _guildId = guildId;
            Thread thread = new Thread(new ThreadStart(GuildQueue));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.AboveNormal;
            thread.Start();
        }
        

        /// <summary>
        /// Adds a commands execution into the queue
        /// </summary>
        /// <param name="task"></param>
        public void Enqueue(IQueueMessage task)
        {
            try
            {
                _logger.Log($"Enqueueing a message for -> {task.context.Guild} <-", LogLevel.Verbose, "MultiThreadQueue:Enqueue");
                _messageQueue.Enqueue(task);
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogLevel.Critical, "MultiThreadQueue:Enqueue");
            }
        }

        /// <summary>
        /// Use with caution. Will destroy the current queue
        /// </summary>
        public void DestroyQueue()
        {
            _destroyQueue = true;
            QueueFactory.GetQueueManager().DeleteGuildQueue(_guildId);
        }

        /// <summary>
        /// Will clear every command from the queue
        /// </summary>
        public void ClearQueue()
        {
            _messageQueue.Clear();
        }

        /// <summary>
        /// Dequeues a message in the queue and returns it for further processing
        /// </summary>
        /// <returns>the first message in the queue or null</returns>
        public IQueueMessage Dequeue()
        {
            try
            {
                _messageQueue.TryDequeue(out IQueueMessage queueMessage);
                return queueMessage;
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogLevel.Critical, "MultiThreadQueue:Dequeue");
                return null;
            }
        }

        /// <summary>
        /// Main method to execute all the commands
        /// </summary>
        private async void GuildQueue()
        {
            try
            {
                await _logger.Log($"Started queue for guild {_guildId}", LogLevel.Debug);
                bool threadAlive = true;
                //keep alive for the guild
                while (threadAlive)
                {
                    //get the current first item in the FiFo queue
                    IQueueMessage currentMessage = Dequeue();
                    if (currentMessage != null)
                    {
                        await _logger.Log($"Dequeued the first message in the queue for guild {currentMessage.context.Guild}", LogLevel.Debug, "MultiThreadQueue:GuildQueue");
                        if (!_destroyQueue)
                        {
                            try
                            {
                                await currentMessage.commands.ExecuteAsync(
                                    context: currentMessage.context,
                                    argPos: currentMessage.argPos,
                                    services: currentMessage.serviceProvider);
                            }
                            catch (Exception e)
                            {
                                await _logger.Log(e.Message, LogLevel.Error, "MultiThreadQueue:GuildQueue:InnerExecution");
                                try
                                {
                                    await currentMessage.context.Channel.SendMessageAsync("😓 Could not execute your command. Please try again, or contact us via our support discord server: https://discord.gg/PeCDUQjS");
                                }
                                catch (Exception e1)
                                {
                                    await _logger.Log(e1.Message, LogLevel.Error, "MultiThreadQueue:GuildQueue:InnerExecution:ReturnMessageError");
                                }
                            }
                        }
                        else
                        {
                            //Destroy the queue
                            threadAlive = false;
                            _messageQueue.Clear();
                            return;
                        }
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "MultiThreadQueue:GuildQueue");
                _messageQueue.Clear();
            }
        }
    }
}
