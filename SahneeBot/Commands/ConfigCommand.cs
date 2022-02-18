using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SahneeBot.Formatter;
using SahneeBot.Tasks;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

[Group("config", "Sahnee bot configuration commands")]
public class ConfigCommand : CommandBase
{
    public ConfigCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
    
    [Group("sahnee-permission", "Configure the permission system")]
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
            [Summary(description: "The role to add the sahnee permission to")]
            IRole role,
            [Summary(description: "The sahnee permission to add")]
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
            [Summary(description: "The role to remove the sahnee permission from")] 
            IRole role,
            [Summary(description: "The sahnee permission to remove - if not specified, all sahnee permissions will be removed")]
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

    [Group("role-config", "Configure the warning roles")]
    public class RoleCommand : CommandBase
    {
        private readonly ChangeRoleColorTask _changeRoleColorTask;
        private readonly RoleColorChangeDiscordFormatter _roleColorChangeDiscordFormatter;
        private readonly GeneralErrorDiscordFormatter _generalErrorDiscordFormatter;

        public RoleCommand(IServiceProvider serviceProvider
            , ChangeRoleColorTask changeRoleColorTask
            , RoleColorChangeDiscordFormatter roleColorChangeDiscordFormatter
            , GeneralErrorDiscordFormatter generalErrorDiscordFormatter) : base(serviceProvider)
        {
            _changeRoleColorTask = changeRoleColorTask;
            _roleColorChangeDiscordFormatter = roleColorChangeDiscordFormatter;
            _generalErrorDiscordFormatter = generalErrorDiscordFormatter;
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
                RequiredRole = RoleType.Administrator
            });
    }
}