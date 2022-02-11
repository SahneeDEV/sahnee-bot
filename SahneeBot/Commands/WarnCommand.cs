using Discord;
using Discord.WebSocket;

namespace SahneeBot.Commands;

public class WarnCommand: ICommand
{
    public SlashCommandBuilder? Build(IGuild? guild)
    {
        if (guild == null)
        {
            return null;
        }
        return new SlashCommandBuilder()
            .WithName("warn")
            .WithDescription("Warns the given user with an optional reason.")
            .AddOption("user", ApplicationCommandOptionType.User, "The user to warn", true)
            .AddOption("reason", ApplicationCommandOptionType.String, "The warning reason");
    }

    public async Task Execute(IGuild? guild, SocketSlashCommand command)
    {
        var opt = command.Data.Options.First(o => o.Name == "reason").Value;
        await command.RespondAsync("Hello World " + opt);
    }
}