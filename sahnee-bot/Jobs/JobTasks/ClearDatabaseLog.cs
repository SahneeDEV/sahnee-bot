using System;
using System.Threading.Tasks;
using sahnee_bot.Database;
using sahnee_bot.Util;

namespace sahnee_bot.Jobs.JobTasks
{
    public class ClearDatabaseLog
    {
        //variables
        private static readonly Logging _logging = new Logging();
        
        public static async Task ClearDatabaseLogAsync()
        {
            try
            {
                await StaticLock.AquireAllAsync();
                try
                {
                    //write the log back to the file and restart the connection
                    StaticDatabase.WriteLogToDatabaseFile();
                    await _logging.LogToConsoleBase("Cleared Database-Log");
                }
                catch (Exception e)
                {
                    await _logging.LogToConsoleBase(e.Message);
                }
            }
            finally
            {
                StaticLock.UnlockAll();
            }
        }
    }
}
