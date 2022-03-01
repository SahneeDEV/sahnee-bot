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
    private readonly int _discordGuildRoleLimitThreshold = 245;

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
        var guildState = await _getGuildStateTask.Execute(context, new GetGuildStateTask.Args(args.GuildId));
        if (guildState.SetRoles)
        {
            //check if the current count is higher than the threshold percent
            if (_discordGuildRoleLimitThreshold <= args.GuildCurrentRoleCount)
            {
                //error
                var channel = (ITextChannel)args.SocketSlashCommand?.Channel!;
                await _informRoleLimitDiscordFormatter.FormatAndSend(
                    new InformRoleLimitDiscordFormatter.Args(args.GuildCurrentRoleCount)
                    ,channel.SendMessageAsync);
                return false;
            }
        }
        return true;
    }
}
