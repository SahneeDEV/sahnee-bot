using System;

namespace sahnee_bot.Database.Schema
{
    public class MasterSchema
    {
        // ReSharper disable once InconsistentNaming
        public ulong _id { get; set; }
        public DateTime Time { get; set; }
        public ulong GuildId { get; set; }
    }
}