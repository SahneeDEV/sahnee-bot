using Discord;
using Discord.Interactions;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;
using SahneeBotModel.Contract;

namespace SahneeBot.Commands;

/// <summary>
/// This command group contains all warning report commands.
/// </summary>
[Group("warnings", "Generates reports about warnings on this Server")]
public class ReportCommand : CommandBase
{
    private const int REPORT_MAX = 100;
    private const int REPORT_MIN = 1;
    private const int REPORT_SINGLE_DEFAULT = 1;
    private const int REPORT_MANY_DEFAULT = 10;
    
    private readonly GetAllWarningsCreatedFromToTask _getAllWarningsCreatedFromToTask;
    private readonly GetRandomWarningsTask _getRandomWarningsTask;
    private readonly NoWarningFoundDiscordFormatter _noWarningFoundDiscordFormatter;
    private readonly WarningDiscordFormatter _warningDiscordFormatter;
    private readonly GetLastWarningsTask _getLastWarningsTask;

    public ReportCommand(
        IServiceProvider serviceProvider, 
        GetAllWarningsCreatedFromToTask getAllWarningsCreatedFromToTask, 
        GetRandomWarningsTask getRandomWarningsTask,
        NoWarningFoundDiscordFormatter noWarningFoundDiscordFormatter,
        WarningDiscordFormatter warningWarningDiscordFormatter,
        GetLastWarningsTask getLastWarningsTask
    ) : base(serviceProvider)
    {
        _getAllWarningsCreatedFromToTask = getAllWarningsCreatedFromToTask;
        _getRandomWarningsTask = getRandomWarningsTask;
        _noWarningFoundDiscordFormatter = noWarningFoundDiscordFormatter;
        _warningDiscordFormatter = warningWarningDiscordFormatter;
        _getLastWarningsTask = getLastWarningsTask;
    }

    [SlashCommand("history", "Gets the warning history of this Server")]
    public Task HistoryCommand(
        [Summary(description: "How long in the past should the bot look? By default only the last 10 warnings will be returned")]
        [MinValue(REPORT_MIN)] [MaxValue(REPORT_MAX)]
        int maxAmount = REPORT_MANY_DEFAULT,
        [Summary(description: "If specified, the warnings will only be chosen from the given user")]
        IUser? user = null
        ) => ExecuteAsync(async ctx =>
    {
        var warnings = await _getLastWarningsTask.Execute(ctx, new GetLastWarningsTask.Args(
            Context.Guild.Id, user?.Id, maxAmount));
        await HelperSendWarnings(ctx, warnings, user);
    });
    
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
        
        var warnings = await HelperGetWarningsBetween(ctx, start, end, user);
        await HelperSendWarnings(ctx, warnings, user);
    });

    [SlashCommand("today", "Gets all warnings that were created today")]
    public Task TodayCommand(
        [Summary(description: "If specified, the warnings will only be chosen from the given user")]
        IUser? user = null
        ) => ExecuteAsync(async ctx =>
    {
        var end = DateTime.UtcNow;
        var start = end - TimeSpan.FromHours(24);
        
        var warnings = await HelperGetWarningsBetween(ctx, start, end, user);
        await HelperSendWarnings(ctx, warnings, user);
    });

    [SlashCommand("random", "Gets random warnings on this Server")]
    public Task RandomCommand(
        [Summary(description: "How many warnings should be returned? By default a single warning will be printed")]
        [MinValue(REPORT_MIN)] [MaxValue(REPORT_MAX)]
        int amount = REPORT_SINGLE_DEFAULT,
        [Summary(description: "If specified, the warnings will only be chosen from the given user")]
        IUser? user = null
    ) => ExecuteAsync(async ctx =>
    {
        var warnings = await _getRandomWarningsTask.Execute(ctx, new GetRandomWarningsTask.Args(
            Context.Guild.Id, user?.Id, amount));
        await HelperSendWarnings(ctx, warnings, user);
    });

    private Task<IEnumerable<IWarning>> HelperGetWarningsBetween(ITaskContext ctx, DateTime start, DateTime end, 
        IUser? user)
    {
        return _getAllWarningsCreatedFromToTask.Execute(ctx, new GetAllWarningsCreatedFromToTask.Args(
            start, end, Context.Guild.Id, user?.Id
        ));
    }

    private async Task HelperSendWarnings(ITaskContext ctx, IEnumerable<IWarning> warnings, IUser? user)
    {
        if (!await _warningDiscordFormatter.FormatAndSendMany(warnings, ModifyOriginalResponseAsync, 
                SendChannelMessageAsync))
        {
            await _noWarningFoundDiscordFormatter.FormatAndSend(
                new NoWarningFoundDiscordFormatter.Args(Context.Guild.Id, user?.Id), ModifyOriginalResponseAsync);
        }
    }
}