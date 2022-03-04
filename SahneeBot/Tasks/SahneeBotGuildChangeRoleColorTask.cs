using System.Text.RegularExpressions;
using ColorHelper;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SahneeBotController;
using SahneeBotController.Tasks;

namespace SahneeBot.Tasks;

public class SahneeBotGuildChangeRoleColorTask: ChangeRoleColorTask
{
    private readonly DiscordSocketClient _bot;
    private const string HEX_REGEX_PATTERN = "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";

    public SahneeBotGuildChangeRoleColorTask(DiscordSocketClient bot)
    {
        _bot = bot;
    }
    
    public override async Task<ISuccess<string>> Execute(ITaskContext ctx, Args arg)
    {
        //Check if the color is valid
        if (string.IsNullOrWhiteSpace(arg.RoleColor) || !Regex.IsMatch(arg.RoleColor, HEX_REGEX_PATTERN))
        {
            return new Error<string>("Please make sure your color string starts with a '#' and is a valid three or six digit hex-code");
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
            .Where(r => r.Name.StartsWith(guildState.WarningRolePrefix));
        try
        {
            var commands = allWarningRolesInGuild.Select(role =>
            {
                var originalColor = role.Color;
                return Command.CreateSimple(
                    () => role.ModifyAsync(r =>
                    {
                        r.Color = customColor;
                    }),
                    () => role.ModifyAsync(r =>
                    {
                        r.Color = originalColor;
                    })
                );
            });
            await Command.DoAll(commands);
        }
        catch (Exception error)
        {
            if (error is HttpException {DiscordCode: DiscordErrorCode.InsufficientPermissions})
            {
                return SahneeBotDiscordError.GetMissingRolePermissionsError<string>(guildState.WarningRolePrefix);
            }
            return new Error<string>(error.Message);
        }

        return new Success<string>(arg.RoleColor);
    }
    
}

