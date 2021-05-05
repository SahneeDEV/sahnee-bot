using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using sahnee_bot.Configuration;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.commands.CommandActions
{
    public class WarnLeaderBoardAction
    {
        //Variables
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Executes the leaderboard message creating
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="Context"></param>
        /// <returns></returns>
        public async Task ExecuteWarnLeaderBoardAsync(uint? amount, SocketCommandContext Context)
        {
            //Variables
            List<string> warnings = new List<string>();
            try
            {
                //Check if an custom amount has been given
                amount ??= StaticConfiguration.GetConfiguration().WarningBot.WarningLeaderboardCount;

                //query all users from the current guild
                List<WarningBotCurrentStatesSchema> currentWarnings = StaticDatabase.WarningCurrentStateCollection()
                    .Query()
                    .Where(g => g.GuildId == Context.Guild.Id)
                    .Limit((int)amount)
                    .ToList();
                
                //insert into dictionary for further processing
                Dictionary<ulong, uint> topWarnings = new Dictionary<ulong, uint>();
                foreach (WarningBotCurrentStatesSchema currentUser in currentWarnings)
                {
                    topWarnings.Add(currentUser.UserId, currentUser.Number);
                }

                //sort the dictionary
                List<KeyValuePair<ulong, uint>> tempList = topWarnings.ToList();
                tempList.Sort((pair, valuePair) => pair.Value.CompareTo(valuePair.Value));
                tempList.Reverse();
                
                //create a message
                warnings = new List<string>();
                string header = $"🏆 Warning Leaderboard 🏆";
                warnings.Add(header);
                string tempMessageBuilder = "";
                uint i = 0;
                foreach (var warning in tempList)
                {
                    //stop if amount is set
                    if (i >= amount)
                    {
                        break;
                    }
                    string msg = "";
                    switch (i)
                    {
                        case 0:
                            msg = $"🥇 <@{warning.Key}> : {warning.Value}\n";
                            break;
                        case 1:
                            msg = $"🥈 <@{warning.Key}> : {warning.Value}\n";
                            break;
                        case 2:
                            msg = $"🥉 <@{warning.Key}> : {warning.Value}\n";
                            break;
                        default:
                            msg = $"💩 <@{warning.Key}> : {warning.Value}\n";
                            break;
                    }
                    //check if we exceed the length with this string
                    if (tempMessageBuilder.Length + msg.Length > StaticInternalConfiguration.CharacterLimitMessageOutbound)
                    {
                        //append the string to the list
                        warnings.Add(tempMessageBuilder);
                        //clear the StringBuilder
                        tempMessageBuilder = "";
                    }
                    tempMessageBuilder += msg;
                    msg = "";
                    i++;
                }
                //add the last messageBuilder to the list
                warnings.Add(tempMessageBuilder);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
            }
            
            //send the message back to the channel
            try
            {
                foreach (var warning in warnings)
                {
                    await Context.Channel.SendMessageAsync(warning);
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "WarnLeaderBoardAction:ExecuteWarnLeaderBoardAsync");
            }
        }
    }
}
