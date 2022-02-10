using System.ComponentModel.DataAnnotations;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class Role : DatabaseObject, IRole
{
    [Required]
    public ulong GuildId { get; set; }
    [Required]
    public string RoleName { get; set; } = "";
    [Required]
    public RoleTypes RoleType { get; set; }
}