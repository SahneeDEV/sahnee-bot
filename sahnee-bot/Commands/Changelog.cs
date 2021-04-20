using System;
using System.Threading.Tasks;
using Discord.Commands;
using sahnee_bot.Configuration;
using sahnee_bot.Logging;
using sahnee_bot.RoleSystem;
using sahnee_bot.Startup;
using sahnee_bot.Util;

namespace sahnee_bot.commands
{
    public class Changelog : ModuleBase<SocketCommandContext>
    {
        //variables
        private readonly Logger _logger = new Logger();

        [Command("changelog")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Will show an amount of changelog entries")]
        public async Task ChangelogAsync()
        {
            try
            {
                //create the message
                ChangeLogParser changeLogParser = new ChangeLogParser();
                string latestChangeLog = await changeLogParser.LatestChangeLog(StaticConfiguration.GetConfiguration().General.ChangeLogPath);
                //send the message to the channel
                await BroadcastLatestChangeLog.SendChangeLogToGuildChannelAsync(Context.Guild, latestChangeLog, Context.Channel);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "Changelog:ChangelogAsync");
            }
        }
        
        [Command("changelog")]
        [RoleHandling(RoleTypes.WarningBotMod)]
        [Summary("Will show an amount of changelog entries")]
        public async Task ChangelogAsync([Summary("custom amount of changelogs to view")]int amount)
        {
            //make negative numbers positive
            if (amount < 0)
            {
                // ReSharper disable once IntVariableOverflowInUncheckedContext
                amount = amount * -1;
            }
            //dont show anything
            if (amount == 0)
            {
                return;
            }
            try
            {
                //create the message
                ChangeLogParser changeLogParser = new ChangeLogParser();
                string latestChangeLog = await changeLogParser.CustomChangeLogLength(StaticConfiguration.GetConfiguration().General.ChangeLogPath, amount);
                //send the message to the channel
                await BroadcastLatestChangeLog.SendChangeLogToGuildChannelAsync(Context.Guild, latestChangeLog, Context.Channel);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "Changelog:ChangelogAsync");
            }
            
        }
    }
}
