namespace sahnee_bot.Database.Schema
{
    public class WarningBotChangeLogSchema : MasterSchema
    {
        public bool Seen { get; set; }
        public string LatestVersion { get; set; }
    }
}
