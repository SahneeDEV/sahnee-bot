using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Commands;

/// <summary>
/// This command group contains all warning report commands.
/// </summary>
[Group("warnings", "Generates reports about warnings on this Server")]
public class ReportCommand : CommandBase
{
    private readonly GetWarningsCreated _task;
    private readonly GetRandomWarningsTask _getRandomWarningsTask;
    private readonly NoWarningFoundDiscordFormatter _noWarningFoundDiscordFormatter;
    private readonly WarningDiscordFormatter _warningDiscordFormatter;

    public ReportCommand(
        IServiceProvider serviceProvider, 
        GetWarningsCreated task, 
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

    [SlashCommand("today", "Gets all warnings, were created today")]
    public Task TodayCommand() => ExecuteAsync(async ctx =>
    {
        var end = DateTime.UtcNow;
        var start = end - TimeSpan.FromHours(24);
        
        var allEmbeds = new List<DiscordFormat>();
        
        var warnings = await _task.Execute(ctx, new GetWarningsCreated.Args(start, end, Context.Guild.Id));
        foreach (var warning in warnings)
        {
            allEmbeds.Add(await _warningDiscordFormatter.Format(warning));
        }

        var embedList = new List<EmbedBuilder>();
        var discordFormats = new List<DiscordFormat>();
        foreach (var embed in allEmbeds)
        {
            if (embed.Embed != null) embedList.Add(embed.Embed);
            if (embedList.Count == 10)
            {
                discordFormats.Add(new DiscordFormat{Embeds = embedList.ToArray()});
            }
        }
        if (embedList.Count > 0) 
        {
            discordFormats.Add(new DiscordFormat{Embeds = embedList.ToArray()});
        }

        for (var i = 0; i < discordFormats.Count; i++)
        {
            var discordFormat = discordFormats[i];
            if (i == 0)
            {
                await discordFormat.Send(ModifyOriginalResponseAsync);
            }
            else
            {
                await discordFormat.Send(RespondAsync);
            }
        }
    });

    [SlashCommand("random", "Gets random warnings on this Server")]
    public Task RandomCommand(
        [Summary(description: "How many warnings should be returned? By default a single warning will be printed")]
        [MinValue(1)] [MaxValue(25)]
        int amount = 1,
        [Summary(description: "If specified, only the warnings will only be chosen from the given user")]
        IUser? user = null
    ) => ExecuteAsync(async ctx =>
    {
        var warnings = (await _getRandomWarningsTask.Execute(ctx, 
            new GetRandomWarningsTask.Args(Context.Guild.Id, user?.Id, amount))).ToArray();
        if (warnings.Length > 0)
        {
            for (var i = 0; i < warnings.Length; i++)
            {
                var warning = warnings[i];
                if (i == 0)
                {
                    await _warningDiscordFormatter.FormatAndSend(warning, ModifyOriginalResponseAsync);
                }
                else
                {
                    await _warningDiscordFormatter.FormatAndSend(warning, SendChannelMessageAsync);
                }
            }
        }
        else
        {
            await _noWarningFoundDiscordFormatter.FormatAndSend(
                new NoWarningFoundDiscordFormatter.Args(Context.Guild.Id, user?.Id), ModifyOriginalResponseAsync);
        }
    });
}