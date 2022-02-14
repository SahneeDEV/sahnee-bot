using System.ComponentModel.DataAnnotations;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class GuildState: IGuildState
{
    [Key, Required]
    public ulong GuildId { get; set; }
    public string? BoundChannel { get; set; }

    public bool SetRoles { get; set; } = true;

    public override string ToString()
    {
        return $"GuildState({nameof(GuildId)}: {GuildId}, {nameof(BoundChannel)}: {BoundChannel}" +
               $", {nameof(SetRoles)}: {SetRoles})";
    }
}