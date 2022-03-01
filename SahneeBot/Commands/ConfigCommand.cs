using Discord;
using Discord.Commands;
using Discord.Interactions;
using SahneeBot.Formatter;
using SahneeBot.InteractionComponents.SelectMenu;
using SahneeBot.Tasks;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

[Discord.Interactions.Group("config", "Sahnee bot configuration commands")]
public class ConfigCommand : CommandBase
{
    public ConfigCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [Discord.Interactions.Group("pm", "Configure private messages about this guild sent to you by the bot")]
    public class PmCommand : CommandBase
    {
        private readonly MessageOptOutTask _optOutTask;
        private readonly GetMessageOptOutTask _getMessageOptOutTask;
        private readonly MessageOptOutDiscordFormatter _messageOptOutDiscordFormatter;

        public PmCommand(
            IServiceProvider serviceProvider,
            MessageOptOutTask optOutTask,
            GetMessageOptOutTask getMessageOptOutTask,
            MessageOptOutDiscordFormatter messageOptOutDiscordFormatter
            ) : base(serviceProvider)
        {
            _optOutTask = optOutTask;
            _getMessageOptOutTask = getMessageOptOutTask;
            _messageOptOutDiscordFormatter = messageOptOutDiscordFormatter;
        }
        
        [SlashCommand("opt-out", "Opts yourself out of receiving messages related to this Server from this bot by private message")]
        public Task OptOutCommand() => ExecuteAsync(async ctx =>
        {
            await HelperOptOut(ctx, true);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            IgnoreBoundChannel = true,
            DeferEphemeral = true
        });
        
        [SlashCommand("opt-in", "Opts yourself back into receiving messages")]
        public Task OptInCommand() => ExecuteAsync(async ctx =>
        {
            await HelperOptOut(ctx, false);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            IgnoreBoundChannel = true,
            DeferEphemeral = true
        });
        
        [SlashCommand("am-i-opted-out", "Checks if you have opted out of receiving messages")]
        public Task IsOptedOutCommand() => ExecuteAsync(async ctx =>
        {
            var optedOut = await _getMessageOptOutTask.Execute(ctx, new GetMessageOptOutTask.Args(
                Context.User.Id, Context.Guild.Id));
            await HelperSendOptOut(ctx, optedOut);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            IgnoreBoundChannel = true,
            DeferEphemeral = true
        });

        private async Task HelperOptOut(ITaskContext ctx, bool optOut)
        {
            var optedOut = await _optOutTask.Execute(ctx, new MessageOptOutTask.Args(Context.User.Id, 
                Context.Guild.Id, optOut));
            await HelperSendOptOut(ctx, optedOut);
        }

        private async Task HelperSendOptOut(ITaskContext ctx, bool optedOut)
        {
            await _messageOptOutDiscordFormatter.FormatAndSend(
                new MessageOptOutDiscordFormatter.Args(Context.User.Id, Context.Guild.Id, optedOut),
                ModifyOriginalResponseAsync, new DiscordFormat.SendOptions
                {
                    Ephemeral = true
                });
        }
    }
    
    [Discord.Interactions.Group("bind", "Configure the bound channel of the bot")]
    public class BindCommand : CommandBase
    {
        private readonly ChangeBoundChannelTask _changeBoundChannelTask;
        private readonly BoundChannelDiscordFormatter _boundChannelDiscordFormatter;
        private readonly GetBoundChannelTask _getBoundChannelTask;

        public BindCommand(IServiceProvider serviceProvider, 
            ChangeBoundChannelTask changeBoundChannelTask,
            BoundChannelDiscordFormatter boundChannelDiscordFormatter,
            GetBoundChannelTask getBoundChannelTask
            ) : base(serviceProvider)
        {
            _changeBoundChannelTask = changeBoundChannelTask;
            _boundChannelDiscordFormatter = boundChannelDiscordFormatter;
            _getBoundChannelTask = getBoundChannelTask;
        }

        [SlashCommand("set", "Binds the bot to a channel")]
        public Task BindChannelCommand(
            [Discord.Interactions.Summary(description: "The channel to bind to")]
            ITextChannel channel
        ) => ExecuteAsync(async ctx =>
        {
            var boundTo = await _changeBoundChannelTask.Execute(ctx, new ChangeBoundChannelTask.Args(
                Context.Guild.Id, channel.Id));
            await _boundChannelDiscordFormatter.FormatAndSend(new BoundChannelDiscordFormatter.Args(Context.Guild.Id,
                boundTo), ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleType.Administrator,
            IgnoreBoundChannel = true
        });

        [SlashCommand("unset", "Unbinds from the channel")]
        public Task UnbindChannelCommand() => ExecuteAsync(async ctx =>
        {
            var boundTo = await _changeBoundChannelTask.Execute(ctx, new ChangeBoundChannelTask.Args(
                Context.Guild.Id, null));
            await _boundChannelDiscordFormatter.FormatAndSend(new BoundChannelDiscordFormatter.Args(Context.Guild.Id,
                boundTo), ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleType.Administrator,
            IgnoreBoundChannel = true
        });

        [SlashCommand("get", "Gets the currently bound channel")]
        public Task GetChannelCommand() => ExecuteAsync(async ctx =>
        {
            var boundTo = await _getBoundChannelTask.Execute(ctx, new GetBoundChannelTask.Args(
                Context.Guild.Id));
            await _boundChannelDiscordFormatter.FormatAndSend(new BoundChannelDiscordFormatter.Args(Context.Guild.Id,
                boundTo), ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleType.Administrator,
            IgnoreBoundChannel = true
        });
    }
    
    [Discord.Interactions.Group("sahnee-permission", "Configure the permission system")]
    public class PermissionCommand : CommandBase
    {
        /// <summary>
        /// Permissions to discord client can interact with. Backing numbers must match with the RoleType enum.
        /// </summary>
        public enum SahneePermission
        {
            Moderator = 0b10,
            Administrator = 0b01
        }
        
        private readonly AddRoleTask _addRoleTask;
        private readonly RemoveRoleTask _removeRoleTask;
        private readonly RoleChangedDiscordFormatter _roleChangedFmt;
        private readonly GetRolesOfGuildTask _rolesOfGuildTask;
        private readonly RoleDiscordFormatter _roleFmt;

        public PermissionCommand(
            IServiceProvider serviceProvider, 
            AddRoleTask addRoleTask, 
            RemoveRoleTask removeRoleTask,
            RoleChangedDiscordFormatter roleChangedFmt,
            GetRolesOfGuildTask rolesOfGuildTask,
            RoleDiscordFormatter roleFmt
            ) : base(serviceProvider)
        {
            _addRoleTask = addRoleTask;
            _removeRoleTask = removeRoleTask;
            _roleChangedFmt = roleChangedFmt;
            _rolesOfGuildTask = rolesOfGuildTask;
            _roleFmt = roleFmt;
        }

        /// <summary>
        /// Adds a role type to a discord role.
        /// </summary>
        /// <param name="role">The discord role.</param>
        /// <param name="sahneePermission">The sahnee permission enum.</param>
        /// <returns>Once the role has been added</returns>
        [SlashCommand("add", "Adds a sahnee permission to a role")]
        public Task CommandAdd(
            [Discord.Interactions.Summary(description: "The role to add the sahnee permission to")]
            IRole role,
            [Discord.Interactions.Summary(description: "The sahnee permission to add")]
            SahneePermission sahneePermission
            ) => ExecuteAsync(async ctx =>
        {
            var roleType = (RoleType)sahneePermission;
            var warnRole = await _addRoleTask.Execute(ctx, new AddRoleTask.Args(Context.Guild.Id, role.Id, 
                roleType));
            await _roleChangedFmt.FormatAndSend(new RoleChangedDiscordFormatter.Args(warnRole, roleType, true), 
                ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleType.Administrator
        });
    
        /// <summary>
        /// Removes a role type from a discord role.
        /// </summary>
        /// <param name="role">The discord role.</param>
        /// <param name="sahneePermission">The sahnee permission or null. If null all role types will be removed</param>
        /// <returns>Once the role has been removed.</returns>
        [SlashCommand("remove", "Removes a (or all) sahnee permission(s) from a role")]
        public Task CommandRemove(
            [Discord.Interactions.Summary(description: "The role to remove the sahnee permission from")] 
            IRole role,
            [Discord.Interactions.Summary(description: "The sahnee permission to remove - if not specified, all sahnee permissions will be removed")]
            SahneePermission? sahneePermission = null
            ) => ExecuteAsync(async ctx =>
        {
            RoleType? roleType = sahneePermission == null ? null : (RoleType)sahneePermission.Value;
            var warnRole = await _removeRoleTask.Execute(ctx, new RemoveRoleTask.Args(Context.Guild.Id, 
                role.Id, roleType));
            await _roleChangedFmt.FormatAndSend(new RoleChangedDiscordFormatter.Args(warnRole, roleType, 
                false), ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleType.Administrator
        });

        /// <summary>
        /// Lists all roles with role types in the guild.
        /// </summary>
        /// <returns>Once all roles have been listed.</returns>
        [SlashCommand("list", "Lists all roles with sahnee permissions on this server")]
        public Task CommandList() => ExecuteAsync(async ctx =>
        {
            var roles = (await _rolesOfGuildTask.Execute(ctx, Context.Guild.Id)).ToArray();
            await ModifyOriginalResponseAsync(msg => msg.Content = new Optional<string>("There are " + roles.Length +
                " roles with sahnee permissions in the Server " + Context.Guild.Name));
            await Task.WhenAll(roles.Select(role => _roleFmt.FormatAndSend(role, SendChannelMessageAsync)));
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleType.Administrator
        });
    }

    [Discord.Interactions.Group("role", "Configure the warning roles")]
    public class RoleCommand : CommandBase
    {
        private readonly ChangeRoleColorTask _changeRoleColorTask;
        private readonly RoleColorChangeDiscordFormatter _roleColorChangeDiscordFormatter;
        private readonly GeneralErrorDiscordFormatter _generalErrorDiscordFormatter;
        private readonly SetGuildRoleSetTask _setGuildRoleSetTask;
        private readonly WarningRoleSetDiscordFormatter _warningRoleSetDiscordFormatter;
        private readonly GetGuildStateTask _getGuildStateTask;
        private readonly SahneeBotChangeWarningRoleNameTask _sahneeBotChangeWarningRoleNameTask;
        private readonly WarningRolePrefixChangedDiscordFormatter _warningRolePrefixChangedDiscordFormatter;

        public RoleCommand(IServiceProvider serviceProvider
            , ChangeRoleColorTask changeRoleColorTask
            , RoleColorChangeDiscordFormatter roleColorChangeDiscordFormatter
            , GeneralErrorDiscordFormatter generalErrorDiscordFormatter
            , SetGuildRoleSetTask setGuildRoleSetTask
            , WarningRoleSetDiscordFormatter warningRoleSetDiscordFormatter
            , GetGuildStateTask getGuildStateTask
            , SahneeBotChangeWarningRoleNameTask sahneeBotChangeWarningRoleNameTask
            , WarningRolePrefixChangedDiscordFormatter warningRolePrefixChangedDiscordFormatter) : base(serviceProvider)
        {
            _changeRoleColorTask = changeRoleColorTask;
            _roleColorChangeDiscordFormatter = roleColorChangeDiscordFormatter;
            _generalErrorDiscordFormatter = generalErrorDiscordFormatter;
            _setGuildRoleSetTask = setGuildRoleSetTask;
            _warningRoleSetDiscordFormatter = warningRoleSetDiscordFormatter;
            _getGuildStateTask = getGuildStateTask;
            _sahneeBotChangeWarningRoleNameTask = sahneeBotChangeWarningRoleNameTask;
            _warningRolePrefixChangedDiscordFormatter = warningRolePrefixChangedDiscordFormatter;
        }

        [SlashCommand("color", "Configure the color of warning roles")]
        public Task ChangeRoleColor(string color)
            => ExecuteAsync(async ctx =>
            {
                var newColor = await _changeRoleColorTask.Execute(ctx
                    , new ChangeRoleColorTask.Args(Context.Guild.Id, color));
                if (string.IsNullOrWhiteSpace(newColor))
                {
                    await _generalErrorDiscordFormatter.FormatAndSend(new GeneralErrorDiscordFormatter.Args(
                        "Cannot set " + color + " as color. Is it a valid hex string?", 
                        new List<EmbedFieldBuilder>
                        {
                            new()
                            {
                                Name = "You specified color",
                                Value = color,
                                IsInline = true
                            },
                            new()
                            {
                                Name = "Hint",
                                Value = "Please make sure, your color string starts with a '#'",
                                IsInline = false
                            }
                        }
                        ), ModifyOriginalResponseAsync);
                    return;
                }
                await _roleColorChangeDiscordFormatter
                    .FormatAndSend(new RoleColorChangeDiscordFormatter.Args(newColor), ModifyOriginalResponseAsync);
            }, new CommandExecutionOptions
            {
                RequiredRole = RoleType.Administrator,
                DeferEphemeral = true,
                PlaceInQueue = true
            });

        [SlashCommand("enable","Enables that warning roles will be set at warning/unwarning")]
        public Task ChangeRoleSetEnable() => ExecuteAsync(async ctx =>
        {
            var guildState = await _setGuildRoleSetTask.Execute(ctx
                , new SetGuildRoleSetTask.Args(Context.Guild.Id, true));
            await _warningRoleSetDiscordFormatter.FormatAndSend(
                new WarningRoleSetDiscordFormatter.Args(guildState.SetRoles
                    , guildState.WarningRolePrefix, true), ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleType.Administrator,
            DeferEphemeral = true
        });
        
        [SlashCommand("disable","Disables that warning roles will be set at warning/unwarning")]
        public Task ChangeRolesSetDisabled() => ExecuteAsync(async ctx =>
        {
            var guildState = await _setGuildRoleSetTask.Execute(ctx
                , new SetGuildRoleSetTask.Args(Context.Guild.Id, false));
            await _warningRoleSetDiscordFormatter.FormatAndSend(
                new WarningRoleSetDiscordFormatter.Args(guildState.SetRoles
                    , guildState.WarningRolePrefix, true), ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleType.Administrator,
            DeferEphemeral = true
        });
        
        [SlashCommand("status","Displays the current status for roles.")]
        public Task ChangeRolesSetStatus() => ExecuteAsync(async ctx =>
        {
            var guildState = await _getGuildStateTask.Execute(ctx, new GetGuildStateTask.Args(Context.Guild.Id));
            await _warningRoleSetDiscordFormatter.FormatAndSend(
                new WarningRoleSetDiscordFormatter.Args(guildState.SetRoles
                    , guildState.WarningRolePrefix, false), ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            RequiredRole = RoleType.Administrator,
            DeferEphemeral = true
        });

        [SlashCommand("prefix","Changes the warning role prefix")]
        public Task ChangeRoleName([Discord.Interactions.Summary(description: "Sets the new prefix of warning roles")]
            string newRolePrefix) => ExecuteAsync(async ctx =>
            {
                var newPrefix = await _sahneeBotChangeWarningRoleNameTask.Execute(ctx
                    , new SahneeBotChangeWarningRoleNameTask.Args(Context.Guild.Id, newRolePrefix));
                await _warningRolePrefixChangedDiscordFormatter.FormatAndSend(
                    new WarningRolePrefixChangedDiscordFormatter.Args(newPrefix)
                    , ModifyOriginalResponseAsync);
            }
        , new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleType.Administrator,
            DeferEphemeral = true
        });
    }

    [Discord.Interactions.Group("old-users", "Manage Users that are not on your guild anymore")]
    public class RemoveUsersCommand : CommandBase
    {
        private readonly SahneeBotGetLeftGuildUsers _sahneeBotGetLeftGuildUsers;
        private readonly RemoveUserFromGuildSelectMenu _removeUserFromGuildSelectMenu;
        private readonly RemovedUsersFromGuildStateDiscordFormatter _removedUsersFromGuildStateDiscordFormatter;

        public RemoveUsersCommand(IServiceProvider serviceProvider
        , SahneeBotGetLeftGuildUsers sahneeBotGetLeftGuildUsers
        , RemoveUserFromGuildSelectMenu removeUserFromGuildSelectMenu
        , RemovedUsersFromGuildStateDiscordFormatter removedUsersFromGuildStateDiscordFormatter) : base(serviceProvider)
        {
            _sahneeBotGetLeftGuildUsers = sahneeBotGetLeftGuildUsers;
            _removeUserFromGuildSelectMenu = removeUserFromGuildSelectMenu;
            _removedUsersFromGuildStateDiscordFormatter = removedUsersFromGuildStateDiscordFormatter;
        }
        
        
        [SlashCommand("remove-list","Gives you a list of Users not on your server anymore" +
                                    ", that can be removed")]
        public Task RemoveOldUsersFromDatabase() => ExecuteAsync(async ctx =>
        {
            var usersToRemove = await _sahneeBotGetLeftGuildUsers.Execute(
                ctx
                , new SahneeBotGetLeftGuildUsers.Args(Context.Guild.Id));
            if (usersToRemove.Count > 0)
            {
                var builder = new ComponentBuilder()
                    .WithSelectMenu(await _removeUserFromGuildSelectMenu.SelectMenu(
                        new RemoveUserFromGuildSelectMenu.Args(usersToRemove)));
                await ModifyOriginalResponseAsync(func: properties =>
                {
                    properties.Content = "Please select all users you want to have removed";
                    properties.Components = builder.Build();
                });
                return;
            }
            await _removedUsersFromGuildStateDiscordFormatter.FormatAndSend(
                new RemovedUsersFromGuildStateDiscordFormatter.Args(Context.Guild.Id, 0, 0)
                , ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            RequiredRole = RoleType.Administrator,
            DeferEphemeral = true
        });

    }
}