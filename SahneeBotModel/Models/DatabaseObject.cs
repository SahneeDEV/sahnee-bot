using System.ComponentModel.DataAnnotations;

namespace SahneeBotModel.Models;

/// <summary>
/// Base class for all Database Objects
/// Provides all child objects with the most basic things
/// </summary>
public class DatabaseObject
{
    /// <summary>
    /// Public Key
    /// </summary>
    [Key, Required]
    public long Id { get; set; }
}
