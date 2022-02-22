using System.ComponentModel.DataAnnotations;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class GuildState: IGuildState
{
    [Key, Required]
    public ulong GuildId { get; set; }
    public ulong? BoundChannelId { get; set; }
    public bool SetRoles { get; set; } = true;
    public string WarningRoleColor { get; set; }
    public Version? LastChangelogVersion { get; set; }

    public override string ToString()
    {
        return $"GuildState({nameof(GuildId)}: {GuildId}, {nameof(BoundChannelId)}: {BoundChannelId}" +
               $", {nameof(SetRoles)}: {SetRoles}, {nameof(WarningRoleColor)}: {WarningRoleColor}, " +
               $"{nameof(LastChangelogVersion)}: {LastChangelogVersion})";
    }
}