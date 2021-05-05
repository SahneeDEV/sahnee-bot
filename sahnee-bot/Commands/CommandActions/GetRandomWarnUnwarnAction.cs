using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.commands.CommandActions
{
    public class GetRandomWarnUnwarnAction
    {
        //variables
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Gets a random warning/unwarn for a user
        /// </summary>
        /// <param name="channel">the channel</param>
        /// <param name="user">the user it's all about</param>
        /// <param name="guild">the current guild</param>
        /// <param name="warningType">the type of warning</param>
        public async Task GetRandomWarnUnwarnActionUserAsync(ISocketMessageChannel channel, IUser user, IGuild guild, WarningType warningType)
        {
            try
            {
                try
                {
                    //get all warnings/unwarns from the user
                    List<WarningBotSchema> allWarnUnwarns = StaticDatabase.WarningCollection()
                        .Query()
                        .Where(u => u.To == user.Id && u.GuildId == guild.Id)
                        .ToList();
                    
                    if (allWarnUnwarns.Count == 0)
                    {
                        await channel.SendMessageAsync("🙄 Well, I don't have anything to present to you.");
                        return;
                    }
                    
                    //get a random warn/unwarn
                    WarningBotSchema warnUnwarn = GetRandomWarnUnwarnUserFromList(allWarnUnwarns, warningType);

                    //check if possibly null
                    if (warnUnwarn == null)
                    {
                        await channel.SendMessageAsync("😕 Could not get a random warning/unwarn");
                        await _logger.Log("Variable warnUnwarn is null!",
                            LogLevel.Error,
                            "GetRandomWarnUnwarnAction:GetRandomWarnUnwarnActionUserAsync:RandomWarnNull");
                    }
                    
                    //print the warn/unwarn
                    //warn the user if there are less than 5 messages, so there can not be a perfect randomness
                    if (allWarnUnwarns.Count < 5)
                    {
                        await channel.SendMessageAsync($"I only have {allWarnUnwarns.Count} records for <@{warnUnwarn.To}. Your results will not be verry random.>");
                    }
                    await channel.SendMessageAsync($"Type: {warnUnwarn.WarningType} From: <@{warnUnwarn.From}> To: <@{warnUnwarn.To}> Reason: {warnUnwarn.Reason}");
                }
                catch (Exception f)
                {
                    await channel.SendMessageAsync("😲 Could not find or get any warning/unwarn for the given user");
                    await _logger.Log(f.Message, LogLevel.Error, "GetRandomWarnUnwarnAction:GetRandomWarnUnwarnActionUserAsync:DBQuery");
                }
                
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "GetRandomWarnAction:GetRandomWarnActionUserAsync");
            }
        }

        /// <summary>
        /// Gets from a random user in a group a random warn/unwarn
        /// </summary>
        /// <param name="channel">the channel</param>
        /// <param name="role">the role it's all about</param>
        /// <param name="guild">the current guild</param>
        /// <param name="warningType">the type of warning</param>
        public async Task GetRandomWarnUnwarnActionGroupAsync(ISocketMessageChannel channel, IRole role, IGuild guild, WarningType warningType)
        {
            try
            {

                //get a random user from the role
                List<IGuildUser> allUsersInGroup = guild.GetUsersAsync(CacheMode.AllowDownload)
                    .Result.Where(u => u.RoleIds.Any(r => r == role.Id)).ToList();
                
                //get a random user from the list of users we got from above
                CryptoRandom random = new CryptoRandom();
                
                IGuildUser theChosenOne = allUsersInGroup[random.Next(0, allUsersInGroup.Count - 1)];
                
                //get all warnings/unwarns from the user
                List<WarningBotSchema> warningsFromChosenOne = StaticDatabase.WarningCollection()
                    .Query()
                    .Where(u => u.To == theChosenOne.Id && u.GuildId == guild.Id)
                    .ToList();

                if (warningsFromChosenOne.Count == 0)
                {
                    await channel.SendMessageAsync("🙄 Well, I don't have anything to present to you.");
                    return;
                }
                
                //get a random warning/unwarn from theChosenOne
                WarningBotSchema warnUnwarn = GetRandomWarnUnwarnUserFromList(warningsFromChosenOne, warningType);
                
                //check if possibly null
                if (warnUnwarn == null)
                {
                    await channel.SendMessageAsync("😕 Could not get a random warning/unwarn");
                    await _logger.Log("Variable warnUnwarn is null!",
                        LogLevel.Error,
                        "GetRandomWarnUnwarnAction:GetRandomWarnUnwarnActionUserAsync:RandomWarnNull");
                }
                    
                //print the warn/unwarn
                //warn the user if there are less than 5 messages, so there can not be a perfect randomness
                if (warningsFromChosenOne.Count < 5)
                {
                    await channel.SendMessageAsync($"I only have {warningsFromChosenOne.Count} records for <@{warnUnwarn.To}. Your results will not be verry random.>");
                }
                await channel.SendMessageAsync($"Type: {warnUnwarn.WarningType} From: <@{warnUnwarn.From}> To: <@{warnUnwarn.To}> Reason: {warnUnwarn.Reason}");
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "GetRandomWarnAction:GetRandomWarnUnwarnActionGroupAsync");
            }
        }

        /// <summary>
        /// Returns a random entriy from a given list with the desired warningtype
        /// </summary>
        /// <param name="allWarnUnwarns">the list of all available warnings</param>
        /// <param name="warningType"></param>
        /// <returns></returns>
        private WarningBotSchema GetRandomWarnUnwarnUserFromList(List<WarningBotSchema> allWarnUnwarns, WarningType warningType)
        {
            try
            {
                bool noMatch = true;
                WarningBotSchema warnUnwarn = new WarningBotSchema();
                CryptoRandom random = new CryptoRandom();
                while (noMatch)
                {
                    //get a random warning/unwarn
                    warnUnwarn = allWarnUnwarns[random.Next(0, allWarnUnwarns.Count - 1)];
                    //check if the warning type matches
                    if (warnUnwarn.WarningType == warningType)
                    {
                        noMatch = false;
                    }
                }
                return warnUnwarn;
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogLevel.Error, "GetRandomWarnAction:GetRandomWarnUnwarnUserFromList");
                return null;
            }
        }
    }
}
