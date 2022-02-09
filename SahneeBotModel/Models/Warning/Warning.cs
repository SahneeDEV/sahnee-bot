using System.ComponentModel.DataAnnotations;

namespace SahneeBotModel.Models.Warning;

public class Warning : DatabaseObject
{
    /// <summary>
    /// Who issued the Warning/Unwarning
    /// </summary>
    [Required]
    public ulong From { get; set; }
    
    /// <summary>
    /// Who got the Warning/Unwarning
    /// </summary>
    [Required]
    public ulong To { get; set; }
    
    /// <summary>
    /// The reason of the Warning/Unwarning
    /// </summary>
    [Required, StringLength(1000), MaxLength(1000)]
    public string Reason { get; set; }
    
    /// <summary>
    /// The Warning/Unwarning number
    /// </summary>
    [Required]
    public ulong Number { get; set; }
    
    /// <summary>
    /// Is it a Warning or an Unwarning
    /// </summary>
    [Required]
    public WarningType WarningType { get; set; }
}
