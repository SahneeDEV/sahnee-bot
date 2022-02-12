﻿using System.ComponentModel.DataAnnotations;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class Warning : DatabaseObject, IWarning
{
    [Required]
    public ulong GuildId { get; set; }
    [Required]
    public ulong UserId { get; set; }
    [Required]
    public DateTime Time { get; set; } = DateTime.UtcNow;
    [Required]
    public ulong IssuerUserId { get; set; }
    [Required, StringLength(1000), MaxLength(1000)]
    public string Reason { get; set; } = "";
    [Required]
    public ulong Number { get; set; }

    public override string ToString()
    {
        return $"Warning({base.ToString()}, {nameof(GuildId)}: {GuildId}, {nameof(UserId)}: {UserId}, {nameof(Time)}: {Time}, {nameof(IssuerUserId)}: {IssuerUserId}, {nameof(Reason)}: {Reason}, {nameof(Number)}: {Number})";
    }
}
