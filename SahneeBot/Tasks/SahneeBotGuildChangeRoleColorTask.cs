using ColorHelper;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotGuildChangeRoleColorTask: ChangeRoleColorTask
{
    private readonly DiscordSocketClient _bot;
    private readonly IConfiguration _configuration;

    public SahneeBotGuildChangeRoleColorTask(DiscordSocketClient bot
        , IConfiguration configuration)
    {
        _bot = bot;
        _configuration = configuration;
    }
    
    public override async Task<string> Execute(ITaskContext ctx, Args arg)
    {
        //Check if the color is valid
        if (!arg.RoleColor.StartsWith('#'))
        {
            //TODO: respond to user, that this is not a valid color
        }
        //set the color in the database
        var guildState = await ctx.Model.GuildStates
            .FirstOrDefaultAsync(g => g.GuildId == arg.GuildId);
        if (guildState == null)
        {
            throw new Exception("Could not set a color to a guild role if that guild does" +
                                " not exist in the database");
        }
        guildState.WarningRoleColor = arg.RoleColor;
        await ctx.Model.SaveChangesAsync();
        //change the color for all warning roles in the current guild
        var rgb = ColorConverter.HexToRgb(new HEX(arg.RoleColor));
        var customColor = new Color(rgb.R, rgb.G, rgb.B);
        //get all warning roles from the guild
        var currentGuild = _bot.GetGuild(arg.GuildId);
        var allWarningRolesInGuild = currentGuild.Roles
            .Where(r => r.Name.StartsWith(_configuration["BotSettings:WarningRolePrefix"]));
        foreach (var currentRole in allWarningRolesInGuild)
        {
            await currentRole.ModifyAsync(r =>
            {
                r.Color = customColor;
            });
        }
        return arg.RoleColor;
    }
    
}

