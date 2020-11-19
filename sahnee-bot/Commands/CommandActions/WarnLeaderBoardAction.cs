using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Util;

namespace sahnee_bot.commands.CommandActions
{
    public class WarnLeaderBoardAction
    {
        //Variables
        private readonly Logging _logging = new Logging();
        
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
                Dictionary<ulong, List<WarningBotSchema>> guildWarnings = new Dictionary<ulong,List<WarningBotSchema>>();
                //a query for every user
                foreach (SocketGuildUser user in Context.Guild.Users)
                {
                    guildWarnings.Add(user.Id, StaticDatabase.WarningCollection().Query()
                        .Where(e => e.GuildId == Context.Guild.Id && e.To == user.Id)
                        .ToList()
                    );
                }
                //get the top x
                Dictionary<ulong, uint> topWarnings = new Dictionary<ulong, uint>();
                foreach (KeyValuePair<ulong, List<WarningBotSchema>> warningUser in guildWarnings)
                {
                    uint currentUserWarnings = (uint) (warningUser.Value.Count - warningUser.Value
                        .FindAll(unwarnings => unwarnings.WarningType == WarningType.Unwarn).Count);
                    topWarnings.Add(warningUser.Key,currentUserWarnings);
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
                await _logging.LogToConsoleBase(e.Message);
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
                await _logging.LogToConsoleBase(e.Message);
            }
        }
    }
}
