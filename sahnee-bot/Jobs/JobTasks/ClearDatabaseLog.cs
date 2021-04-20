using System;
using System.Threading.Tasks;
using sahnee_bot.Database;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.Jobs.JobTasks
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ClearDatabaseLog
    {
        //variables
        private static readonly Logger _logger = new Logger();
        
        public static async Task ClearDatabaseLogAsync()
        {
            try
            {
                await StaticLock.AquireAllAsync();
                try
                {
                    //write the log back to the file and restart the connection
                    StaticDatabase.WriteLogToDatabaseFile();
                    await _logger.Log("Cleared Database-Log", LogLevel.Info);
                }
                catch (Exception e)
                {
                    await _logger.Log(e.Message, LogLevel.Critical, "ClearDatabaseLog:ClearDatabaseLogAsync");
                }
            }
            finally
            {
                StaticLock.UnlockAll();
            }
        }
    }
}
