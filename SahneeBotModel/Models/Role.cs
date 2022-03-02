using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

[Index(nameof(GuildId), nameof(RoleId))]
[Index(nameof(GuildId))]
[Index(nameof(RoleId))]
public class Role : IRole
{
    [Required]
    public ulong GuildId { get; set; }
    [Required]
    public ulong RoleId { get; set; }
    [Required]
    public RoleType RoleType { get; set; }

    public override string ToString()
    {
        return $"Role({base.ToString()}, {nameof(GuildId)}: {GuildId}, {nameof(RoleId)}: {RoleId}, " +
               $"{nameof(RoleType)}: {RoleType})";
    }
}