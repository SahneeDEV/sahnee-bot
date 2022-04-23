namespace SahneeBot; 

/// <summary>
/// Contains string helper functions.
/// </summary>
public static class StringExtensions {
    /// <summary>
    /// Limits the length of a string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="maxLength">The max length.</param>
    /// <returns>The limited string.</returns>
    public static string MaxLength(this string str, int maxLength) {
        return str[..Math.Min(str.Length, maxLength)];
    }
}