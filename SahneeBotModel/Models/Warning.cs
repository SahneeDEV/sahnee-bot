using System.ComponentModel.DataAnnotations;
using SahneeBotModel.Contract;

namespace SahneeBotModel.Models;

public class Warning : DatabaseObject, IWarning
{
    [Required]
    public ulong From { get; set; }
    [Required]
    public ulong To { get; set; }
    [Required, StringLength(1000), MaxLength(1000)]
    public string Reason { get; set; } = "";
    [Required]
    public ulong Number { get; set; }
    [Required]
    public WarningType WarningType { get; set; }
    [Required]
    public ulong GuildId { get; set; }
    [Required]
    public DateTime Time { get; set; }
}
