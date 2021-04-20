using System;
using Discord.Commands;

namespace sahnee_bot.Queue
{
    public class BasicQueueMessage : IQueueMessage
    {
        public SocketCommandContext context { get; set; }
        public int argPos { get; set; }
        public IServiceProvider serviceProvider { get; set; }
        public CommandService commands { get; set; }
    }
}
