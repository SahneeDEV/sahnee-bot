using Discord;
using Discord.WebSocket;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

/// <summary>
/// Checks if the guild role limit got hit, and will notify the guild if so
/// </summary>
public class SahneeBotRoleLimitInformationTask : ITask<SahneeBotRoleLimitInformationTask.Args, bool>
{
    private readonly GetGuildStateTask _getGuildStateTask;
    private readonly InformRoleLimitDiscordFormatter _informRoleLimitDiscordFormatter;
    private const int DISCORD_GUILD_ROLE_LIMIT_THRESHOLD = 245;

    public record struct Args(int GuildCurrentRoleCount, ulong GuildId, SocketSlashCommand? SocketSlashCommand);

    public SahneeBotRoleLimitInformationTask(GetGuildStateTask getGuildStateTask
        , InformRoleLimitDiscordFormatter informRoleLimitDiscordFormatter)
    {
        _getGuildStateTask = getGuildStateTask;
        _informRoleLimitDiscordFormatter = informRoleLimitDiscordFormatter;
    }

    /// <summary>
    /// Returns if a guild is close to reach the discord role limit
    /// </summary>
    /// <param name="context">the current context</param>
    /// <param name="args">all arguments</param>
    /// <returns>true for yes, false for no</returns>
    public async Task<bool> Execute(ITaskContext context, Args args)
    {
        var (guildCurrentRoleCount, guildId, socketSlashCommand) = args;
        var guildState = await _getGuildStateTask.Execute(context, new GetGuildStateTask.Args(guildId));
        if (guildState.SetRoles)
        {
            //check if the current count is higher than the threshold percent
            if (DISCORD_GUILD_ROLE_LIMIT_THRESHOLD <= guildCurrentRoleCount)
            {
                //error
                var channel = (ITextChannel)socketSlashCommand?.Channel!;
                await _informRoleLimitDiscordFormatter.FormatAndSend(
                    new InformRoleLimitDiscordFormatter.Args(guildCurrentRoleCount)
                    , channel.SendMessageAsync);
                return false;
            }
        }
        return true;
    }
}
