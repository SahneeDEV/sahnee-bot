using System;
using sahnee_bot.Logging;

namespace sahnee_bot.Configuration
{
    public static class StaticConfiguration
    {
        //Variables
        private static Configuration _configuration;

        /// <summary>
        /// Loads the current configuration upon calling
        /// </summary>
        static StaticConfiguration()
        {
            try
            {
                LoadConfiguration loadConfiguration = new LoadConfiguration(true);
                _configuration = loadConfiguration.GetConfiguration();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Returns the current configuration
        /// </summary>
        /// <returns></returns>
        public static Configuration GetConfiguration()
        {
            return _configuration;
        }

        /// <summary>
        /// Dynamically rereads the configuration file and updates it's cache
        /// </summary>
        public static void ReReadConfiguration()
        {
            //reread the configuration
            try
            {
                LoadConfiguration loadConfiguration = new LoadConfiguration(true);
                _configuration = loadConfiguration.GetConfiguration();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
