using SahneeBotModel;

namespace SahneeBot.Formatter;

/// <summary>
/// Prints a hint that a permission is missing.
/// </summary>
public class MissingPermissionDiscordFormatter: IDiscordFormatter<MissingPermissionDiscordFormatter.Args>
{
    /// <summary>
    /// Arguments for sending the missing permission hint.
    /// </summary>
    /// <param name="MissingRole">The role that is required.</param>
    public record struct Args(RoleType MissingRole);

    public Task<DiscordFormat> Format(Args arg)
    {
        return Task.FromResult(new DiscordFormat($"Missing permission {arg.MissingRole} for this command."));
    }
}