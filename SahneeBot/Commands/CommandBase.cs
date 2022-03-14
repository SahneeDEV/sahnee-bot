using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBot.Formatter;
using SahneeBot.Formatter.Error;
using SahneeBot.Tasks;
using SahneeBot.Tasks.Error;
using SahneeBotController;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

public delegate Task CommandDelegate(ITaskContext ctx);

public delegate Task<ISuccess> CommandSuccessDelegate(ITaskContext ctx);

/// <summary>
/// Base class for commands.
/// </summary>
public abstract class CommandBase : InteractionModuleBase<IInteractionContext>
{
    /// <summary>
    /// The service provider of the command.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    private readonly GetRolesOfUserTask _roles;
    private readonly MissingPermissionDiscordFormatter _missingPermFmt;
    private readonly ExceptionDiscordFormatter _exceptionFmt;
    private readonly ErrorDiscordFormatter _errorFmt;
    private readonly SahneeBotReportExceptionTask _exceptionTask;
    private readonly GetBoundChannelTask _boundChannel;
    private readonly NotBoundChannelDiscordFormatter _notBoundChannelFmt;
    private readonly SahneeBotTaskContextFactory _contextFactory;

    /// <summary>
    /// Creates a new command base class.
    /// </summary>
    /// <param name="serviceProvider">The provider to use for DI.</param>
    protected CommandBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        // We resolve all further classes manually instead of injection to keep the ctor simple for inheritance.
        _roles = serviceProvider.GetRequiredService<GetRolesOfUserTask>();
        _missingPermFmt = serviceProvider.GetRequiredService<MissingPermissionDiscordFormatter>();
        _exceptionFmt = serviceProvider.GetRequiredService<ExceptionDiscordFormatter>();
        _errorFmt = serviceProvider.GetRequiredService<ErrorDiscordFormatter>();
        _boundChannel = serviceProvider.GetRequiredService<GetBoundChannelTask>();
        _notBoundChannelFmt = serviceProvider.GetRequiredService<NotBoundChannelDiscordFormatter>();
        _exceptionTask = serviceProvider.GetRequiredService<SahneeBotReportExceptionTask>();
        _contextFactory = serviceProvider.GetRequiredService<SahneeBotTaskContextFactory>();
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

        /// <summary>
        /// Ignore the channel the bot has been bound to.
        /// </summary>
        public readonly bool IgnoreBoundChannel { get; init; }
    }

    /// <summary>
    /// Deletes the original message after the given amount of MS.
    /// </summary>
    /// <param name="ms">The MS.</param>
    protected void DeleteOriginalResponseAfter(int ms = 5000)
    {
#pragma warning disable CS4014
        DeleteOriginalResponseAfterAndWaitAsync(ms);
#pragma warning restore CS4014
    }

    /// <summary>
    /// Deletes the original message after the given amount of MS and allows to await the deletion.
    /// </summary>
    /// <param name="ms">The MS.</param>
    protected Task DeleteOriginalResponseAfterAndWaitAsync(int ms = 5000)
    {
        return Task.Delay(ms).ContinueWith(task => DeleteOriginalResponseAsync());
    }

    protected Task ExecuteAsync(CommandDelegate del, CommandExecutionOptions opts = default)
    {
        async Task<ISuccess> WrapperFunction(ITaskContext ctx)
        {
            await del(ctx);
            return new Success<bool>(true);
        }

        return ExecuteAsync(WrapperFunction, opts);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="del">The command delegate.</param>
    /// <param name="opts">Options to customize command execution.</param>
    protected async Task ExecuteAsync(CommandSuccessDelegate del, CommandExecutionOptions opts = default)
    {
        ulong? placeInQueue = null;

        // Execute command immediately or place in queue
        if (opts.PlaceInQueue)
        {
            placeInQueue = Context.Guild?.Id;
            if (!placeInQueue.HasValue)
            {
                await ReplyAsync("This command cannot be used outside of a server.");
                throw new InvalidOperationException("Cannot place global commands in a guild queue");
            }
        }

        // Give us more than three seconds to respond
        await DeferAsync(opts.DeferEphemeral, opts.DeferRequest);

        async Task<ISuccess> ExecuteImpl(ITaskContext ctx)
        {
            // Check binding
            if (!opts.IgnoreBoundChannel)
            {
                var channel = Channel;
                if (channel == null)
                {
                    throw new InvalidOperationException(
                        "Cannot ignore bound channel when the interaction does not use channels");
                }

                if (Context.Guild == null)
                {
                    throw new InvalidOperationException("Cannot ignore bound channel in a global command");
                }

                var boundId = await _boundChannel.Execute(ctx, new GetBoundChannelTask.Args(Context.Guild.Id));
                if (boundId.HasValue && boundId.Value != channel.Id)
                {
                    await _notBoundChannelFmt.FormatAndSend(
                        new NotBoundChannelDiscordFormatter.Args(Context.Guild.Id, boundId),
                        ModifyOriginalResponseAsync);
                    DeleteOriginalResponseAfter();
                    return new Error<bool>("Incorrect channel");
                }
            }

            // Check permission
            if (opts.RequiredRole.HasValue && opts.RequiredRole.Value != RoleType.None)
            {
                var role = opts.RequiredRole.Value;
                if (Context.Guild == null)
                {
                    throw new InvalidOperationException("Cannot check guild permission in a global command");
                }

                var allowed = await _roles.HasRoleAsync(ctx,
                    new GetRolesOfUserTask.Args(Context.Guild.Id, Context.User.Id), role);
                if (!allowed)
                {
                    await _missingPermFmt.FormatAndSend(new MissingPermissionDiscordFormatter.Args(role),
                        ModifyOriginalResponseAsync);
                    DeleteOriginalResponseAfter();
                    return new Error<bool>("Missing permission");
                }
            }

            return await del(ctx);
        }

        async Task ErrorReporter(ITaskContext ctx, SahneeBotTaskContextFactory.ErrorReport report
            , SahneeBotTaskContextFactory.ContextOptions ctxOpts)
        {
            var (crash, error) = report;
            if (crash != null)
            {
                await _exceptionFmt.FormatAndSend(
                    new ExceptionDiscordFormatter.Args(ctxOpts.Type, ctxOpts.Name,
                        ctxOpts.Debug, Context.Guild?.Id, Context.User?.Id, crash.Value.Exception
                        , crash.Value.TicketId, false)
                    , ModifyOriginalResponseAsync);
            }
            else if (error != null)
            {
                await _errorFmt.FormatAndSend(new ErrorDiscordFormatter.Args(ctxOpts.Type, ctxOpts.Name,
                        ctxOpts.Debug, Context.Guild?.Id, Context.User?.Id, error, false)
                , ModifyOriginalResponseAsync);
            }
        }

        await _contextFactory.ExecuteWithContextAsync(
            ExecuteImpl
            , new SahneeBotTaskContextFactory.ContextOptions
            {
                Type = "slash-command",
                Name = GetDebugString((SocketSlashCommand) Context.Interaction),
                Debug = GetDebugString((SocketSlashCommand) Context.Interaction, true),
                RelatedGuildId = Context.Guild?.Id,
                RelatedUserId = Context.User?.Id,
                PlaceInQueue = placeInQueue,
                ErrorReporter = ErrorReporter
            });
    }

    protected ITextChannel? Channel
    {
        get
        {
            var interaction = Context.Interaction as SocketSlashCommand;
            return interaction?.Channel as ITextChannel;
        }
    }

    /// <summary>
    /// The delegate to send a message in the current channel.
    /// </summary>
    protected DiscordFormat.SendChannelMessageAsyncDelegate SendChannelMessageAsync
    {
        get
        {
            var channel = Channel;
            if (channel == null)
            {
                throw new InvalidOperationException("Cannot get channel in this kind of interaction");
            }

            return channel.SendMessageAsync;
        }
    }

    private static string GetDebugString(IDiscordInteraction interaction, bool sensitive = false)
    {
        var sb = new StringBuilder();
        if (interaction is ISlashCommandInteraction slashInteraction)
        {
            sb.Append('/');
            sb.Append(slashInteraction.Data.Name);
            if (sensitive)
            {
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
        }
        else
        {
            sb.Append(interaction);
        }

        return sb.ToString();
    }
}