using System.Collections.Concurrent;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SahneeBot.Commands;

public class CommandHandler: ICommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<CommandHandler> _logger;

    private readonly IDictionary<ulong, IDictionary<ulong, ICommand>> _commands =
        new ConcurrentDictionary<ulong, IDictionary<ulong, ICommand>>();

    private readonly IList<ICommand> _commandClasses = new List<ICommand>();

    public CommandHandler(DiscordSocketClient client, ILogger<CommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public void Install()
    {
        _logger.LogInformation(EventIds.Startup, "Creating commands...");
        var types = DiscoverCommandTypes();
        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is not ICommand commandClass)
            {
                continue;
            }
            _logger.LogDebug(EventIds.Startup, "Created command: {name}", type.Name);
            _commandClasses.Add(commandClass);
        }
        _client.SlashCommandExecuted += SlashCommandHandler;
        //_client.Ready += Ready;
        _client.GuildAvailable += GuildAvailable;
    }

    private async Task GuildAvailable(SocketGuild arg)
    {
        await InstallCommandsOnGuild(arg);
    }

    /*private async Task Ready()
    {
        var tasks = _client.Guilds.Select(InstallCommandsOnGuild);
        await Task.WhenAll(tasks);
    }*/
    
    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        var channel = (SocketGuildChannel)command.Channel;
        var guild = channel.Guild;
        var guildInfo = _commands[guild.Id];
        var commandInfo = guildInfo[command.CommandId];
        await commandInfo.Execute(guild, command);
    }

    /// <summary>
    /// Installs all bot commands on the given guild.
    /// </summary>
    /// <param name="guild">The guild to install the commands on.</param>
    /// <returns>The installed commands.</returns>
    private async Task InstallCommandsOnGuild(SocketGuild guild)
    {
        _logger.LogDebug(EventIds.Startup, "Installing commands on guild {name}", guild.Name);
        var commands = _commandClasses
            .Select(commandClass => commandClass.Build(guild))
            .Where(builder => builder != null)
            .Select(builder => builder!.Build() as ApplicationCommandProperties)
            .ToArray();
        var registeredCommands = await guild.BulkOverwriteApplicationCommandAsync(commands);
        var dict = new Dictionary<ulong, ICommand>();
        var index = 0;
        foreach (var registeredCommand in registeredCommands)
        {
            var commandClass = _commandClasses[index];
            dict.Add(registeredCommand.Id, commandClass);
            index++;
        }
        _commands[guild.Id] = dict;
    }

    /// <summary>
    /// Finds all types that implement the ICommand interface.
    /// </summary>
    /// <returns>An enumerable of the types.</returns>
    private static IEnumerable<Type> DiscoverCommandTypes()
    {
        var type = typeof(ICommand);
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p))
            .Where(p => p.IsClass);
    }

    /*private class GuildReturnInfo<T>
    {
        public readonly IGuild Guild;
        public readonly T Result;

        public GuildReturnInfo(IGuild guild, T result)
        {
            Guild = guild;
            Result = result;
        }
    }*/
}
