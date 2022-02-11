using System.ComponentModel.DataAnnotations;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class UserGuildState: IUserGuildState
{
    [Required]
    public ulong GuildId { get; set; }
    [Required]
    public ulong UserId { get; set; }
    [Required]
    public uint WarningNumber { get; set; }
    [Required]
    public bool MessageOptOut { get; set; }
    [Required]
    public bool HasReceivedOptOutHint { get; set; }
}
