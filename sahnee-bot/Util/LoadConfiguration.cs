using System;
using System.IO;
using Newtonsoft.Json;

namespace sahnee_bot.Util
{
    public class LoadConfiguration
    {
        //Variables
        private Configuration _configuration;
        private readonly string _configurationPath = Directory.GetCurrentDirectory() + "/config.json";
        private bool _configurationLoaded = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LoadConfiguration()
        {
            
        }

        /// <summary>
        /// Determine if you want to deserialize the config at the beginning or just want to create the object without
        /// deserialization.
        /// </summary>
        /// <param name="loadDirectly"> True for direct deserialization. False for later/manual deserialization</param>
        public LoadConfiguration(bool loadDirectly)
        {
            if (loadDirectly)
            {
                //Load the configuration
                LoadConfig();
            }
        }

        /// <summary>
        /// Freshly loads and deserializes the configuration
        /// You need this Method if you did not load the configuration with initializing the class.
        /// </summary>
        /// <returns>JsonObject containing the current configuration</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public Configuration LoadAndDeserialize()
        {
            //Check if the file exists
            if (!File.Exists(_configurationPath))
            {
                throw new FileNotFoundException();
            }
            //read the file
            string fileContent = File.ReadAllText(_configurationPath);
            //Deserialize the content into the JsonObject
            Configuration jsonObject = JsonConvert.DeserializeObject<Configuration>(fileContent);
            _configurationLoaded = true;
            return jsonObject;
        }

        /// <summary>
        /// Returns the current configuration JsonObject
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Configuration GetConfiguration()
        {
            if (_configurationLoaded)
            {
                return _configuration;
            }
            throw new InvalidCastException();
        }


        /// <summary>
        /// Loads the configuration file in the JSON format and deserializes it
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        private void LoadConfig()
        {
            //Check if the file exists
            if (!File.Exists(_configurationPath))
            {
                throw new FileNotFoundException();
            }
            //read the file
            string fileContent = File.ReadAllText(_configurationPath);
            //Deserialize the content into the JsonObject
            _configuration = JsonConvert.DeserializeObject<Configuration>(fileContent);
            _configurationLoaded = true;
        }
    }
}
