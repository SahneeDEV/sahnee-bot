using Discord.Net;
using SahneeBotController;
using SahneeBotController.Tasks;
using Discord;
using System.Net;

namespace SahneeBot;

/// <summary>
/// Class for converting discord errors into our ISuccess.
/// </summary>
public class SahneeBotDiscordError
{
    private readonly GetGuildStateTask _getGuildStateTask;
    
    public SahneeBotDiscordError(GetGuildStateTask getGuildStateTask)
    {
        _getGuildStateTask = getGuildStateTask;
    }

    /// <summary>
    /// Options for reporting the error.
    /// </summary>
    public struct ErrorOptions
    {
        /// <summary>
        /// The guild ID.
        /// </summary>
        public ulong? GuildId { get; init; }
        
        /// <summary>
        /// The exception.
        /// </summary>
        public Exception Exception { get; init; }
    }

    /// <summary>
    /// Returns the error as an ISuccess if it can be easily handled by the application.
    /// </summary>
    /// <param name="ctx">The context.</param>
    /// <param name="options">Report options.</param>
    /// <typeparam name="T">The success type.</typeparam>
    /// <returns>If the error can easily be handled an Error, otherwise null meaning you should rethrow.</returns>
    public async Task<ISuccess<T>?> TryGetError<T>(ITaskContext ctx, ErrorOptions options)
    {
        switch (options.Exception)
        {
            // Aggregate exception
            case AggregateException aggregateException:
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    var innerReportOptions = options with
                    {
                        Exception = innerException
                    };
                    var innerSuccess = await TryGetError<T>(ctx, innerReportOptions);
                    if (innerSuccess != null)
                    {
                        return innerSuccess;
                    }
                }

                return null;
            }
            // Bot is below managed role
            case HttpException {DiscordCode: DiscordErrorCode.InsufficientPermissions}:
            {
                var prefix = await GetGuildPrefix(ctx, options.GuildId);
                return GetMissingRolePermissionsError<T>(prefix);
            }
            // Bot cannot manage roles
            case HttpException {HttpCode: HttpStatusCode.Forbidden}:
            {
                var prefix = await GetGuildPrefix(ctx, options.GuildId);
                return GetMissingRolePermissionsError<T>(prefix);
            }
            default:
            {
                return null;
            }
        }
    }

    private async Task<string> GetGuildPrefix(ITaskContext ctx, ulong? guildId)
    {
        var guildState = guildId.HasValue
            ? await _getGuildStateTask.Execute(ctx, new GetGuildStateTask.Args(guildId.Value))
            : null;
        return guildState?.WarningRolePrefix.Trim() ?? "n/a";
    }
    
    private static ISuccess<T> GetMissingRolePermissionsError<T>(string prefix)
    {
        return new Error<T>("The Sahnee-Bot does not have the required permissions to edit the warning roles on your " +
                            "server. Please drag the Sahnee-Bot role above all other roles starting with " +
                            $"\"{prefix.TrimEnd()}\" in your Server Settings and make sure that it has the \"Manage " +
                            "Roles\" permission.");
    }

    /*private static ISuccess<T> GetMissingBotPermissionError<T>(string permission)
    {
        return new Error<T>("The Sahnee-Bot does not have the required permissions to " + permission + " on your " +
                            "server. Please edit the permissions of the Sahnee-Bot in your Server Settings and try " +
                            "again.");
    }*/
}