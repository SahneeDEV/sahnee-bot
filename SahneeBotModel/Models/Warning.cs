using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

[Index(nameof(UserId))]
[Index(nameof(GuildId))]
[Index(nameof(GuildId), nameof(Time))]
[Index(nameof(GuildId), nameof(UserId))]
[Index(nameof(GuildId), nameof(UserId), nameof(Time))]
[Index(nameof(GuildId), nameof(IssuerUserId))]
[Index(nameof(GuildId), nameof(IssuerUserId), nameof(Time))]
public class Warning : DatabaseObject, IWarning
{
    [Required] public ulong GuildId { get; set; }
    [Required] public ulong UserId { get; set; }
    [Required] public DateTime Time { get; set; } = DateTime.UtcNow;
    [Required] public ulong IssuerUserId { get; set; }
    [Required, StringLength(1000), MaxLength(1000)] public string Reason { get; set; } = "";
    [Required] public ulong Number { get; set; }
    [Required] public WarningType Type { get; set; } = WarningType.Warning;

    public override string ToString()
    {
        return $"Warning({base.ToString()}, {nameof(GuildId)}: {GuildId}, {nameof(UserId)}: {UserId}, " +
               $"{nameof(Time)}: {Time}, {nameof(IssuerUserId)}: {IssuerUserId}, {nameof(Reason)}: {Reason}, " +
               $"{nameof(Number)}: {Number}, {nameof(Type)}: {Type})";
    }
}
