using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using sahnee_bot.Logging;

namespace sahnee_bot.Util
{
    public class ChangeLogParser
    {
        //Variables
        private readonly string _logDivider = "#%&";
        private static readonly Logger _logger = new Logger();
        
        /// <summary>
        /// Reads the changelog file and returns the latest changes
        /// </summary>
        /// <param name="logFileLocation">location of the changelog file</param>
        /// <returns></returns>
        public async Task<string> LatestChangeLog(string logFileLocation)
        {
            string logMessage = "";
            try
            {
                //create the reader to read the file
                using (StreamReader reader = new StreamReader(logFileLocation, Encoding.UTF8))
                {
                    bool keepReading = true;
                    //only read until the _logDivider
                    while (keepReading)
                    {
                        string tempStream = await reader.ReadLineAsync();
                        if (tempStream == _logDivider)
                        {
                            keepReading = false;
                        }
                        else
                        {
                            logMessage +=  tempStream + "\n";
                        }
                    }
                }
                return logMessage;
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
                return "Could not read the changelog.";
            }
        }
        
        /// <summary>
        /// Reads the changelog file and returns a custom amount of changes from the changelog
        /// </summary>
        /// <param name="logFileLocation">location of the changelog file</param>
        /// <param name="amount">the amount of entries to view</param>
        /// <returns></returns>
        public async Task<string> CustomChangeLogLength(string logFileLocation, int amount)
        {
            string logMessage = "";
            try
            {
                //create the reader to read through the file
                using (StreamReader reader = new StreamReader(logFileLocation, Encoding.UTF8))
                {
                    bool keepReading = true;
                    int amountOfCycles = 0;
                    //read until the amount of cycles has been made or ran out of text
                    while (keepReading)
                    {
                        string tempStream = await reader.ReadLineAsync();
                        if (tempStream == _logDivider)
                        {
                            amountOfCycles++;
                            if (amountOfCycles >= amount)
                            {
                                //End of cycles
                                keepReading = false;
                            }
                        }
                        else if (tempStream == null || reader.EndOfStream)
                        {
                            //EOF
                            keepReading = false;
                        }
                        else
                        {
                            logMessage += tempStream + "\n";
                        }
                    }
                }
                return logMessage;
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error);
                return "Could not read the changelog.";
            }
        }
    }
}
