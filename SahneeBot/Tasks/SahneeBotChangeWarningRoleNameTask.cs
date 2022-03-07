using Discord;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotChangeWarningRoleNameTask : ChangeWarningRoleNameTask
{
    private readonly Bot _bot;
    private readonly SahneeBotDiscordError _discordError;

    public SahneeBotChangeWarningRoleNameTask(IServiceProvider provider
        , Bot bot
        , SahneeBotDiscordError discordError) : base(provider)
    {
        _bot = bot;
        _discordError = discordError;
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
        catch (Exception exception)
        {
            var error = await _discordError.TryGetError<string>(ctx, new SahneeBotDiscordError.ErrorOptions
            {
                Exception = exception,
                GuildId = args.GuildId
            });
            if (error != null)
            {
                return error;
            }
            throw;
        }

        return oldPrefix;
    }
}
