using System;

namespace sahnee_bot.Database.Schema
{
    public class WarningBotSchema : MasterSchema
    {
        public ulong From { get; set; }
        public ulong To { get; set; }
        public string Reason { get; set; }
        public ulong Number { get; set; }
        public WarningType WarningType { get; set; }
    }
}
