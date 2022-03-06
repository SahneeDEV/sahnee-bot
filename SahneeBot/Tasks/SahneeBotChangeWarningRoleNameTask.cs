using Discord;
using Discord.Net;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotChangeWarningRoleNameTask : ChangeWarningRoleNameTask
{
    private readonly Bot _bot;

    public SahneeBotChangeWarningRoleNameTask(IServiceProvider provider
        , Bot bot) : base(provider)
    {
        _bot = bot;
    }

    public override async Task<ISuccess<string>> Execute(ITaskContext ctx, Args args)
    {
        var oldPrefix = await base.Execute(ctx, args);

        if (!oldPrefix.IsSuccess)
        {
            return oldPrefix;
        }
        
        //change the warning roles in the current guild
        var currentGuild = await _bot.Client.GetGuildAsync(args.GuildId);
        var allWarningRoles = currentGuild.Roles.Where(r => r.Name.StartsWith(oldPrefix.Value));
        if (!args.WarningRolePrefix.EndsWith(" "))
        {
            args.WarningRolePrefix += " ";
        }

        try
        {
            var commands = allWarningRoles.Select(role =>
            {
                var originalName = role.Name;
                return Command.CreateSimple(
                    () => role.ModifyAsync(r =>
                    {
                        r.Name = originalName.Replace(oldPrefix.Value, args.WarningRolePrefix);
                    }),
                    () => role.ModifyAsync(r =>
                    {
                        r.Name = originalName;
                    })
                );
            });
            await Command.DoAll(commands);
        }
        catch (Exception error)
        {
            if (error is HttpException {DiscordCode: DiscordErrorCode.InsufficientPermissions})
            {
                return SahneeBotDiscordError.GetMissingRolePermissionsError<string>(oldPrefix.Value);
            }
            return new Error<string>(error.Message);
        }

        return oldPrefix;
    }
}
