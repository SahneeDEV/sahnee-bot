using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json;
using sahnee_bot.Configuration;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.OtherAPI.DiscordBotList
{
    public class DiscordBotList : MainApi
    {
        //variables
        private readonly Logger _logger = new Logger();

        /// <summary>
        /// Will updates the amount of guilds an the amount of users for this bot at https://discordbotlist.com/bots/sahnee-bot
        /// </summary>
        /// <returns></returns>
        public async Task SendStatisticsAsync()
        {
            //Sends the amount of guilds first
            await SendGuildAmountAsync();
        }

        /// <summary>
        /// Will update the amount of guild this bot currently is in at https://discordbotlist.com/bots/sahnee-bot
        /// </summary>
        /// <returns></returns>
        public async Task SendGuildAmountAsync()
        {
            try
            {
                //create a new http client
                HttpClient client = new HttpClient();
                string url = StaticConfiguration.GetConfiguration().ExternalApi.DiscordBotList.ApiUrl
                             + "/bots/" + StaticConfiguration.GetConfiguration().ExternalApi.DiscordBotList.BotId
                             + "/stats";
                //get the current amount of guilds for the bot
                DiscordSocketClient bot = StaticBot.GetBot() as DiscordSocketClient;
                JsonObject apiObject;
                if (bot != null)
                {
                    apiObject = new JsonObject()
                    {
                        guilds = bot.Guilds.Count
                    };
                }
                else
                {
                    await _logger.Log("Bot could not be initialized to communicate to the DiscordBotList API.", LogLevel.Warning, "DiscordBotList:SendGuildAmountAsync");
                    return;
                }
                
                //serialize the object to json
                string apiJson = JsonConvert.SerializeObject(apiObject);
                //set the body
                StringContent data = new StringContent(apiJson, Encoding.UTF8, "application/json");
                //authorization
                //check for a key, if not just abort
                if (StaticConfiguration.GetConfiguration().ExternalApi.DiscordBotList.AuthToken.Length <= 1)
                {
                    return;
                }
                client.DefaultRequestHeaders.Add("Authorization", StaticConfiguration.GetConfiguration().ExternalApi.DiscordBotList.AuthToken);
                //make the http POST
                await client.PostAsync(url, data);
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "DiscordBotList:SendGuildAmountAsync");
            }
        }
    }
}
