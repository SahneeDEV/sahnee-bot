using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to get all Warnings of the current day of all users
/// </summary>
public class WarningsTodayCommand: CommandBase
{
    private readonly GetWarningsCreated _task;
    private readonly ILogger<WarningsTodayCommand> _logger;
    private readonly WarningDiscordFormatter _warningDiscordFormatter;

    public WarningsTodayCommand(
        IServiceProvider serviceProvider, 
        GetWarningsCreated task, 
        ILogger<WarningsTodayCommand> logger,
        WarningDiscordFormatter warningWarningDiscordFormatter
    ) : base(serviceProvider)
    {
        _task = task;
        _logger = logger;
        _warningDiscordFormatter = warningWarningDiscordFormatter;
    }

    [SlashCommand("warnings-today", "Gets all warnings, were created today")]
    public Task WarningsToday() => ExecuteAsync(async ctx =>
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
}