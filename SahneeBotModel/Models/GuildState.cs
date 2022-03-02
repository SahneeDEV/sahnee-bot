using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

[Index(nameof(GuildId), nameof(SetRoles))]
[Index(nameof(GuildId), nameof(WarningRolePrefix))]
[Index(nameof(GuildId), nameof(WarningRoleColor))]
[Index(nameof(GuildId), nameof(BoundChannelId))]
public class GuildState: IGuildState
{
    [Key, Required]
    public ulong GuildId { get; set; }
    public ulong? BoundChannelId { get; set; }
    public bool SetRoles { get; set; } = true;
    public string WarningRoleColor { get; set; } = "";
    public Version? LastChangelogVersion { get; set; }
    public string WarningRolePrefix { get; set; } = "";

    public override string ToString()
    {
        return $"GuildState({nameof(GuildId)}: {GuildId}, {nameof(BoundChannelId)}: {BoundChannelId}" +
               $", {nameof(SetRoles)}: {SetRoles}, {nameof(WarningRoleColor)}: {WarningRoleColor}, " +
               $"{nameof(LastChangelogVersion)}: {LastChangelogVersion}, {nameof(WarningRolePrefix)}: " +
               $"{WarningRolePrefix})";
    }
}