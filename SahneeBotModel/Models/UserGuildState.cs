using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class UserGuildState: IUserGuildState
{
    [Required] public ulong GuildId { get; set; }
    [Required] public ulong UserId { get; set; }
    [Required] public uint WarningNumber { get; set; } = 0;
    [Required] public bool MessageOptOut { get; set; } = false;
    [Required] public bool HasReceivedOptOutHint { get; set; } = false;

    public override string ToString()
    {
        return $"UserGuildState({nameof(GuildId)}: {GuildId}, {nameof(UserId)}: {UserId}, " +
               $"{nameof(WarningNumber)}: {WarningNumber}, {nameof(MessageOptOut)}: {MessageOptOut}, " +
               $"{nameof(HasReceivedOptOutHint)}: {HasReceivedOptOutHint})";
    }
}
