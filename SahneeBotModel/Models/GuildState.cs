using System.ComponentModel.DataAnnotations;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class GuildState: IGuildState
{
    [Key, Required]
    public ulong GuildId { get; set; }
    public string? BoundChannel { get; set; }
}