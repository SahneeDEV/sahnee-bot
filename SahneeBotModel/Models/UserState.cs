﻿using System.ComponentModel.DataAnnotations;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class UserState: IUserState
{
    [Key, Required]
    public ulong UserId { get; set; }
    public DateTime? LastDataDeletion { get; set; }
}
