using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

[Index(nameof(UserId))]
public class UserState: IUserState
{
    [Key, Required]
    public ulong UserId { get; set; }
    public DateTime? LastDataDeletion { get; set; }

    public override string ToString()
    {
        return $"UserState({nameof(UserId)}: {UserId}, {nameof(LastDataDeletion)}: {LastDataDeletion})";
    }
}
