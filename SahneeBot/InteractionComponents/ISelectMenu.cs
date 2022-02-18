namespace SahneeBot.InteractionComponents;

public interface ISelectMenu
{
    /// <summary>
    /// The placeholder for the select menu
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// The minimum amount of select menu options the user will have to select
    /// </summary>
    public int MinValues { get; set; }
    
    /// <summary>
    /// The maximum amount of select menu options the user can select
    /// </summary>
    public int MaxValues { get; set; }
    
    /// <summary>
    /// The unique custom id for the select menu
    /// </summary>
    public string CustomId { get; set; }
    
    /// <summary>
    /// Determines whether the select menu is disabled or not
    /// </summary>
    public bool IsDisabled { get; set; }
}
