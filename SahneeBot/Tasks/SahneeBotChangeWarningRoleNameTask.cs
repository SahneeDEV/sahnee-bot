using Discord.WebSocket;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

/// <summary>
/// Changes the warning role name of the given guild in the guild state
/// </summary>
public class SahneeBotChangeWarningRoleNameTask : ITask<SahneeBotChangeWarningRoleNameTask.Args, string>
{
    private readonly ChangeWarningRoleNameTask _changeWarningRoleNameTask;
    private readonly DiscordSocketClient _bot;

    public record struct Args(ulong GuildId, string WarningRolePrefix);

    public SahneeBotChangeWarningRoleNameTask(ChangeWarningRoleNameTask changeWarningRoleNameTask,
        DiscordSocketClient bot)
    {
        _changeWarningRoleNameTask = changeWarningRoleNameTask;
        _bot = bot;
    }

    public async Task<string> Execute(ITaskContext ctx, Args args)
    {
        //change the prefix in the guildState
        var oldPrefix = await _changeWarningRoleNameTask.Execute(ctx
            , new ChangeWarningRoleNameTask.Args(args.GuildId, args.WarningRolePrefix));
        
        //change the warning roles in the current guild
        var currentGuild = _bot.GetGuild(args.GuildId);
        var allWarningRoles = currentGuild.Roles.Where(r => r.Name.StartsWith(oldPrefix));
        if (!args.WarningRolePrefix.EndsWith(" "))
        {
            args.WarningRolePrefix += " ";
        }
        foreach (var currentRole in allWarningRoles)
        {
            if (currentRole != null)
            {
                await currentRole.ModifyAsync(r =>
                {
                    r.Name = currentRole.Name.Replace(oldPrefix, args.WarningRolePrefix);
                });
            }
        }
        return args.WarningRolePrefix;
    }
}
