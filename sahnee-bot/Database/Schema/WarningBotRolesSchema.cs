using sahnee_bot.RoleSystem;

namespace sahnee_bot.Database.Schema
{
    public class WarningBotRolesSchema : MasterSchema
    {
        public string RoleName { get; set; }
        public ulong RoleId { get; set; }
        public RoleTypes RoleType { get; set; }
    }
}
