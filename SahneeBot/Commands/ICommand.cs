using Discord;
using Discord.WebSocket;

namespace SahneeBot.Commands;

/// <summary>
/// Interface for creating commands.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Builds the command.
    /// </summary>
    /// <param name="guild">The guild to build the command for.</param>
    /// <param name="builder">The command builder.</param>
    SlashCommandBuilder? Build(IGuild? guild);

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="guild">The guild the command is executed on.</param>
    /// <param name="command">The command.</param>
    Task Execute(IGuild? guild, SocketSlashCommand command);
}