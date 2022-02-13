using Discord;
using Discord.Interactions;
using SahneeBot.Formatter;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to give additional information about the bot.
/// </summary>
public class HelpCommand: CommandBase
{
   
    private readonly HelpDiscordFormatter _discordFormatter;

    public HelpCommand(HelpDiscordFormatter discordFormatter, IServiceProvider serviceProvider): base(serviceProvider)
    {
        _discordFormatter = discordFormatter;
    }

    /// <summary>
    /// The actual help command.
    /// </summary>
    [SlashCommand("help", "Gives a details about the bot")]
    public Task Help() => ExecuteAsync(async ctx =>
    {
        await _discordFormatter.FormatAndSend(new HelpDiscordFormatter.Args(), ModifyOriginalResponseAsync);
    });
}