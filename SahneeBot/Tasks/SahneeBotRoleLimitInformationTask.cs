using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotRoleLimitInformationTask
{
    private readonly GetGuildStateTask _getGuildStateTask;
    private readonly InformRoleLimitDiscordFormatter _informRoleLimitDiscordFormatter;
    private readonly int _discordGuildRoleLimitThreshold = 245;

    public record struct Args(int GuildCurrentRoleCount, ulong GuildId);

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
    /// <param name="socketSlashCommand">the current socket slash command</param>
    /// <param name="args">all arguments</param>
    /// <returns>true for yes, false for no</returns>
    public async Task CheckGuildRoleLimit(ITaskContext context, SocketSlashCommand? socketSlashCommand, Args args)
    {
        var guildState = await _getGuildStateTask.Execute(context, new GetGuildStateTask.Args(args.GuildId));
        if (guildState.SetRoles)
        {
            //check if the current count is higher than the threshold percent
            if (_discordGuildRoleLimitThreshold <= args.GuildCurrentRoleCount)
            {
                //error
                var channel = (ITextChannel)socketSlashCommand?.Channel!;
                await _informRoleLimitDiscordFormatter.FormatAndSend(
                    new InformRoleLimitDiscordFormatter.Args(args.GuildCurrentRoleCount)
                    ,channel.SendMessageAsync);
            }
        }
    }
}
