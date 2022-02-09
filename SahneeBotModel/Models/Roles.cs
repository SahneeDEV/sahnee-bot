using System.ComponentModel.DataAnnotations;

namespace SahneeBotModel.Models;

public class Roles : DatabaseObject
{
    /// <summary>
    /// The Name of the Role
    /// </summary>
    [Required]
    public string RoleName { get; set; }
    
    /// <summary>
    /// The Type of the Role
    /// </summary>
    [Required]
    public RoleTypes RoleType { get; set; }
}