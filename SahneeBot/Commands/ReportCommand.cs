using Discord;
using Discord.Interactions;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Commands;

/// <summary>
/// This command group contains all warning report commands.
/// </summary>
[Group("warnings", "Generates reports about warnings on this Server")]
public class ReportCommand : CommandBase
{
    private readonly GetAllWarningsCreatedFromToTask _task;
    private readonly GetRandomWarningsTask _getRandomWarningsTask;
    private readonly NoWarningFoundDiscordFormatter _noWarningFoundDiscordFormatter;
    private readonly WarningDiscordFormatter _warningDiscordFormatter;

    public ReportCommand(
        IServiceProvider serviceProvider, 
        GetAllWarningsCreatedFromToTask task, 
        GetRandomWarningsTask getRandomWarningsTask,
        NoWarningFoundDiscordFormatter noWarningFoundDiscordFormatter,
        WarningDiscordFormatter warningWarningDiscordFormatter
    ) : base(serviceProvider)
    {
        _task = task;
        _getRandomWarningsTask = getRandomWarningsTask;
        _noWarningFoundDiscordFormatter = noWarningFoundDiscordFormatter;
        _warningDiscordFormatter = warningWarningDiscordFormatter;
    }
    
    [SlashCommand("between", "Gets all warnings in the given time frame")]
    public Task BetweenCommand(
        [Summary("start", "The start of the time frame")]
        DateTime startRaw,
        [Summary("end", "The end of the time frame - If not specified, the current date and time will be used")]
        DateTime? endRaw = null,
        [Summary(description: "If specified, the warnings will only be chosen from the given user")]
        IUser? user = null
    ) => ExecuteAsync(async ctx =>
    {
        var start = startRaw.ToUniversalTime();
        var end = endRaw?.ToUniversalTime() ?? DateTime.UtcNow;
        
        var warnings = await _task.Execute(ctx, new GetAllWarningsCreatedFromToTask.Args(
            start, end, Context.Guild.Id, user?.Id
        ));
        
        if (!await _warningDiscordFormatter.FormatAndSendMany(
                warnings, 
                ModifyOriginalResponseAsync, 
                SendChannelMessageAsync
            ))
        {
            await _noWarningFoundDiscordFormatter.FormatAndSend(new NoWarningFoundDiscordFormatter.Args(
                Context.Guild.Id, 
                user?.Id
            ), ModifyOriginalResponseAsync);
        }
    });

    [SlashCommand("today", "Gets all warnings that were created today")]
    public Task TodayCommand(
        [Summary(description: "If specified, the warnings will only be chosen from the given user")]
        IUser? user = null
        ) => ExecuteAsync(async ctx =>
    {
        var end = DateTime.UtcNow;
        var start = end - TimeSpan.FromHours(24);
        
        var warnings = await _task.Execute(ctx, new GetAllWarningsCreatedFromToTask.Args(
            start, end, Context.Guild.Id, user?.Id
            ));
        
        if (!await _warningDiscordFormatter.FormatAndSendMany(
            warnings, 
            ModifyOriginalResponseAsync, 
            SendChannelMessageAsync
            ))
        {
            await _noWarningFoundDiscordFormatter.FormatAndSend(new NoWarningFoundDiscordFormatter.Args(
                Context.Guild.Id, 
                user?.Id
                ), ModifyOriginalResponseAsync);
        }
    });

    [SlashCommand("random", "Gets random warnings on this Server")]
    public Task RandomCommand(
        [Summary(description: "How many warnings should be returned? By default a single warning will be printed")]
        [MinValue(1)] [MaxValue(25)]
        int amount = 1,
        [Summary(description: "If specified, the warnings will only be chosen from the given user")]
        IUser? user = null
    ) => ExecuteAsync(async ctx =>
    {
        var warnings = (await _getRandomWarningsTask.Execute(ctx, 
            new GetRandomWarningsTask.Args(Context.Guild.Id, user?.Id, amount)));
        if (!await _warningDiscordFormatter.FormatAndSendMany(warnings, ModifyOriginalResponseAsync, 
                SendChannelMessageAsync))
        {
            await _noWarningFoundDiscordFormatter.FormatAndSend(
                new NoWarningFoundDiscordFormatter.Args(Context.Guild.Id, user?.Id), ModifyOriginalResponseAsync);
        }
    });
}