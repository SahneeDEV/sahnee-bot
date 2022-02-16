using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

public delegate Task CommandDelegate(ITaskContext ctx);

/// <summary>
/// Base class for commands.
/// </summary>
public abstract class CommandBase: InteractionModuleBase<IInteractionContext>
{
    /// <summary>
    /// The service provider of the command.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;
    
    private readonly ILogger<CommandBase> _logger;
    private readonly GuildQueue _queue;
    private readonly GetRolesOfUserTask _roles;
    private readonly MissingPermissionDiscordFormatter _missingPermFmt;
    private readonly CommandErrorDiscordFormatter _errorFmt;

    /// <summary>
    /// Creates a new command base class.
    /// </summary>
    /// <param name="serviceProvider">The provider to use for DI.</param>
    protected CommandBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        // We resolve all further classes manually instead of injection to keep the ctor simple for inheritance.
        _logger = serviceProvider.GetRequiredService<ILogger<CommandBase>>();
        _queue = serviceProvider.GetRequiredService<GuildQueue>();
        _roles = serviceProvider.GetRequiredService<GetRolesOfUserTask>();
        _missingPermFmt = serviceProvider.GetRequiredService<MissingPermissionDiscordFormatter>();
        _errorFmt = serviceProvider.GetRequiredService<CommandErrorDiscordFormatter>();
    }
    
    /// <summary>
    /// Options for executing a command.
    /// </summary>
    public record struct CommandExecutionOptions
    {
        /// <summary>
        /// Should the command be placed in the guild queue?
        /// </summary>
        public readonly bool PlaceInQueue { get; init; }
        /// <summary>
        /// Is the defer response ephemeral?
        /// </summary>
        public readonly bool DeferEphemeral { get; init; }
        /// <summary>
        /// The request options of the defer response
        /// </summary>
        public readonly RequestOptions? DeferRequest { get; init; }
        /// <summary>
        /// The role require for this command.
        /// </summary>
        public readonly RoleType? RequiredRole { get; init; }
    }
    
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="del">The command delegate.</param>
    /// <param name="opts">Options to customize command execution.</param>
    protected async Task ExecuteAsync(CommandDelegate del, CommandExecutionOptions opts = default)
    {
        var scope = ServiceProvider.CreateScope();
        await DeferAsync(opts.DeferEphemeral, opts.DeferRequest);
        
        async Task ExecuteAsyncImpl()
        {
            try
            {
                // Create context
                await using var model = scope.ServiceProvider.GetRequiredService<SahneeBotModelContext>();
                await using var transaction = await model.Database.BeginTransactionAsync();
                using var ctx = new SahneeBotTaskContext(scope.ServiceProvider, scope, model, transaction);
                // Check permission
                if (opts.RequiredRole.HasValue && opts.RequiredRole.Value != RoleType.None)
                {
                    var role = opts.RequiredRole.Value;
                    if (Context.Guild == null)
                    {
                        throw new InvalidOperationException("Cannot check guild permission in a global command");
                    }
                    var allowed = await _roles.HasRoleAsync(ctx, new GetRolesOfUserTask.Args(Context.Guild.Id, 
                            Context.User.Id), role);
                    if (!allowed)
                    {
                        await _missingPermFmt.FormatAndSend(new MissingPermissionDiscordFormatter.Args(role),
                            ModifyOriginalResponseAsync);
#pragma warning disable CS4014
                        Task.Delay(5000).ContinueWith(task => DeleteOriginalResponseAsync());
#pragma warning restore CS4014
                        return;
                    }
                }
                // Run command
                await del(ctx);
                // Commit transaction
                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                var interaction = (SocketSlashCommand) Context.Interaction;
                await _errorFmt.FormatAndSend(
                    new CommandErrorDiscordFormatter.Args(interaction.CommandName, 
                        GetDebugString(interaction), Context.Guild?.Id, Context.User.Id, exception), 
                    ModifyOriginalResponseAsync);
            }
            scope.Dispose();
        }

        if (opts.PlaceInQueue)
        {
            var guildId = Context.Guild?.Id;
            if (!guildId.HasValue)
            {
                scope.Dispose();
                throw new InvalidOperationException("Cannot place global commands in a guild queue");
            }
            _queue.Enqueue(guildId.Value, ExecuteAsyncImpl);
        }
        else
        {
            await ExecuteAsyncImpl();
        }
    }

    /// <summary>
    /// The delegate to send a message in the current channel.
    /// </summary>
    protected DiscordFormat.SendChannelMessageAsyncDelegate SendChannelMessageAsync
    {
        get
        {
            var interaction = (SocketSlashCommand)Context.Interaction;
            var channel = (ITextChannel)interaction.Channel;
            return channel.SendMessageAsync;
        }
    }

    private static string GetDebugString(IDiscordInteraction interaction)
    {
        var sb = new StringBuilder();
        if (interaction is ISlashCommandInteraction slashInteraction)
        {
            sb.Append('/');
            sb.Append(slashInteraction.Data.Name);
            foreach (var option in slashInteraction.Data.Options)
            {
                sb.Append(" <");
                sb.Append(option.Name);
                sb.Append(':');
                sb.Append(option.Value);
                sb.Append('>');
            }

            sb.Append(" (#");
            sb.Append(slashInteraction.Data.Id);
            sb.Append(')');
        }
        else
        {
            sb.Append(interaction);
        }
        return sb.ToString();
    }
}
