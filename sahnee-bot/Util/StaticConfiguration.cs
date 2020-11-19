using System;

namespace sahnee_bot.Util
{
    public static class StaticConfiguration
    {
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
                Logging logging = new Logging();
                logging.LogToConsoleBase(e.Message);
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
                Logging logging = new Logging();
                logging.LogToConsoleBase(e.Message);
            }
        }
    }
}
