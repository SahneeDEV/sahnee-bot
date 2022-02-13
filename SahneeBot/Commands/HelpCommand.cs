using Discord;
using Discord.Interactions;
using SahneeBot.Formatter;

namespace SahneeBot.Commands;

/// <summary>
/// This command is used to give additional information about the bot.
/// </summary>
public class HelpCommand: InteractionModuleBase<IInteractionContext>
{
   
    private readonly HelpDiscordFormatter _discordFormatter;

    public HelpCommand(HelpDiscordFormatter discordFormatter)
    {
        _discordFormatter = discordFormatter;
    }
    
    /// <summary>
    /// The actual help command.
    /// </summary>
    [SlashCommand("help", "Gives a details about the bot")]
    public async Task Help()
    {
        
        try
        {
            await _discordFormatter.FormatAndSend(new HelpDiscordFormatter.Args(), RespondAsync);
        }
        catch (Exception e)
        {
           // _logger.LogWarning(EventIds.Command, e, "Failed to send warning message: {Warning}", 
           //     warning);
        }
    }
}