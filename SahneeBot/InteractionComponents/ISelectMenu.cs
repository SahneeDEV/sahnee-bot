using Discord;

namespace SahneeBot.InteractionComponents;

/// <summary>
/// A select menu class for data.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISelectMenu<in T>
{
    /// <summary>
    /// Select menu builder
    /// </summary>
    /// <param name="arg">The data to insert into the select Menu</param>
    /// <returns></returns>
    Task<SelectMenuBuilder> SelectMenu(T arg);
}
