using System.ComponentModel.DataAnnotations;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class UserState: IUserState
{
    [Key, Required]
    public ulong GuildId { get; set; }
    [Key, Required]
    public ulong UserId { get; set; }
    [Required]
    public uint WarningNumber { get; set; }
}
