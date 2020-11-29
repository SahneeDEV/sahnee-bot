﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace sahnee_bot.Util
{
    public class ChangeLogParser
    {
        //Variables
        private readonly string _logDivider = "#%&";
        private static readonly Logging _logging = new Logging();
        
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
                            logMessage+=  tempStream + "\n";
                        }
                    }
                }
                return logMessage;
            }
            catch (Exception e)
            {
                await _logging.LogToConsoleBase(e.Message);
                return "Could not read the changelog.";
            }
        }
    }
}
