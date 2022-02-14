using Discord;
using Discord.Interactions;
using SahneeBot.Formatter;
using SahneeBotController.Tasks;
using SahneeBotModel;

namespace SahneeBot.Commands;

[Group("config", "Sahnee bot configuration commands")]
public class ConfigCommand : CommandBase
{
    public ConfigCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
    
    [Group("roles", "Configure the permission system")]
    public class PermissionCommand : CommandBase
    {
        private readonly AddRoleTask _addRoleTask;
        private readonly RemoveRoleTask _removeRoleTask;
        private readonly RoleDiscordFormatter _roleFmt;

        public PermissionCommand(
            IServiceProvider serviceProvider, 
            AddRoleTask addRoleTask, 
            RemoveRoleTask removeRoleTask,
            RoleDiscordFormatter roleFmt
            ) : base(serviceProvider)
        {
            _addRoleTask = addRoleTask;
            _removeRoleTask = removeRoleTask;
            _roleFmt = roleFmt;
        }

        [SlashCommand("add", "Adds a role")]
        public Task CommandAdd(
            [Summary(description: "The role to add")] IRole role,
            [Summary(description: "The role type to add")] RoleTypes roleType
        ) => ExecuteAsync(async ctx =>
        {
            var warnRole = await _addRoleTask.Execute(ctx, new AddRoleTask.Args(Context.Guild.Id, role.Name, 
                roleType));
            await _roleFmt.FormatAndSend(warnRole, ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleTypes.Administrator
        });

        [SlashCommand("remove", "Removes a role")]
        public Task CommandRemove(
            [Summary(description: "The role to remove")] IRole role
        ) => ExecuteAsync(async ctx =>
        {
            var warnRole = await _removeRoleTask.Execute(ctx, new RemoveRoleTask.Args(Context.Guild.Id, role.Name));
            if (warnRole == null)
            {
                
            }
            await _roleFmt.FormatAndSend(warnRole, ModifyOriginalResponseAsync);
        }, new CommandExecutionOptions
        {
            PlaceInQueue = true,
            RequiredRole = RoleTypes.Administrator
        });
    }
}