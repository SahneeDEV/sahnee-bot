using Discord;
using Discord.Interactions;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Commands;

public class RandomWarningCommand : CommandBase
{
    private readonly GetRandomWarningTask _getRandomWarningTask;
    private readonly WarningDiscordFormatter _warningDiscordFormatter;
    private readonly NoWarningFoundDiscordFormatter _noWarningFoundDiscordFormatter;

    public RandomWarningCommand(
        IServiceProvider serviceProvider,
        GetRandomWarningTask getRandomWarningTask,
        WarningDiscordFormatter warningDiscordFormatter,
        NoWarningFoundDiscordFormatter noWarningFoundDiscordFormatter
    ) : base(serviceProvider)
    {
        _getRandomWarningTask = getRandomWarningTask;
        _warningDiscordFormatter = warningDiscordFormatter;
        _noWarningFoundDiscordFormatter = noWarningFoundDiscordFormatter;
    }

    [SlashCommand("random-warning", "Gets a random warning on this Server")]
    public Task Command(
        [Summary(description: "If specified, gets a random warning of the given user on this Server")]
        IUser? user = null
        ) => ExecuteAsync(async ctx =>
    {
        var warning = await _getRandomWarningTask.Execute(ctx, new GetRandomWarningTask.Args(Context.Guild.Id,
            user?.Id));
        if (warning != null)
        {
            await _warningDiscordFormatter.FormatAndSend(warning, ModifyOriginalResponseAsync);
        }
        else
        {
            await _noWarningFoundDiscordFormatter.FormatAndSend(
                new NoWarningFoundDiscordFormatter.Args(Context.Guild.Id, user?.Id), ModifyOriginalResponseAsync);
        }
    });
}