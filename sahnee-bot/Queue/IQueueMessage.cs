using System;
using Discord.Commands;

namespace sahnee_bot.Queue
{
    /// <summary>
    /// Represents a generic queue message
    /// </summary>
    public interface IQueueMessage
    {
        //Variables
        public SocketCommandContext context { get; set; }
        public int argPos { get; set; }
        public IServiceProvider serviceProvider { get; set; }
        public CommandService commands { get; set; }
    }
}
