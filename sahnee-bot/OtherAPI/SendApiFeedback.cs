using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using sahnee_bot.Logging;
using sahnee_bot.OtherAPI;

namespace sahnee_bot.OtherAPI
{
    public class SendApiFeedback
    {
        //Variables
        private static List<MainApi> _availableApis = new List<MainApi>();
        private static Logger _logger = new Logger();

        /// <summary>
        /// Adds an available api to the list for execution
        /// </summary>
        /// <param name="apiFunctionToCall"></param>
        public static void AddAvailableApi(MainApi apiFunctionToCall)
        {
            _availableApis.Add(apiFunctionToCall);
        }
        
        /// <summary>
        /// Main function to call every added external api
        /// </summary>
        /// <param name="guild">guild or null</param>
        /// <returns></returns>
        public static async Task SendApiFeedbackAsync(IGuild guild)
        {
            try
            {
                foreach (MainApi availableApi in _availableApis)
                {
                    try
                    {
                        await availableApi.SendStatisticsAsync();
                    }
                    catch (Exception e)
                    {
                        await _logger.Log(e.Message, LogLevel.Error, $"SendApiFeedback:AddAvailableApi:{availableApi.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "SendApiFeedback:AddAvailableApi");
            }
        }
    }
}
