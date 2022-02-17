using Discord;
using Discord.Interactions;

namespace SahneeBot.Commands;

public class TestCommand: InteractionModuleBase<IInteractionContext>
{

    [SlashCommand("test", "Testing only")]
    public async Task Test()
    {
        await RespondAsync("Tescht");
    }
}