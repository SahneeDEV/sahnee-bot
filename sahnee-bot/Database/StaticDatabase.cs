using System;
using LiteDB;
using sahnee_bot.Configuration;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;
using sahnee_bot.Startup.StartupActions;

namespace sahnee_bot.Database
{
    public static class StaticDatabase
    {
        //Variables
        private static LiteDatabase _dataBase;
        private static readonly Logger _logger = new Logger();

        /// <summary>
        /// Collections
        /// </summary>
        private static ILiteCollection<WarningBotSchema> _warningCollection;
        private static ILiteCollection<WarningBotCurrentStatesSchema> _warningCurrentStateCollection;
        private static ILiteCollection<WarningBotChangeLogSchema> _warningChangeLogCollection;
        private static ILiteCollection<WarningBotPrefixSchema> _warningPrefixCollection;
        private static ILiteCollection<WarningBotRolesSchema> _warningRolesCollection;

        /// <summary>
        /// Loads a instance of the database
        /// </summary>
        public static void LoadDatabase()
        {
            //get the database or create if it doesn't exist
            _dataBase = new LiteDatabase(StaticConfiguration.GetConfiguration().General.DatabasePath);
            //get or create all necessary collections
            //warning table
            _warningCollection = 
                _dataBase.GetCollection<WarningBotSchema>(
                    StaticConfiguration.GetConfiguration().WarningBot.DatabaseCollection);
            //warning state table
            _warningCurrentStateCollection = 
                _dataBase.GetCollection<WarningBotCurrentStatesSchema>(
                    StaticConfiguration.GetConfiguration().WarningBot.DatabaseCollection + "_state");
            //change log state table
            _warningChangeLogCollection = 
                _dataBase.GetCollection<WarningBotChangeLogSchema>(
                    StaticConfiguration.GetConfiguration().WarningBot.DatabaseCollection + "_changelog");
            //warning prefix table
            _warningPrefixCollection = 
                _dataBase.GetCollection<WarningBotPrefixSchema>(
                    StaticConfiguration.GetConfiguration().WarningBot.DatabaseCollection + "_prefix");
            //warning roles table
            _warningRolesCollection =
                _dataBase.GetCollection<WarningBotRolesSchema>(
                    StaticConfiguration.GetConfiguration().WarningBot.DatabaseCollection + "_roles");
            
            //migrations
            DatabaseMigrations.DoDatabaseMigrations();
        }

        /// <summary>
        /// Returns the current Database Object
        /// </summary>
        /// <returns></returns>
        public static LiteDatabase GetDatabase()
        {
            return _dataBase;
        }

        /// <summary>
        /// Returns the warningBot collection
        /// </summary>
        /// <returns></returns>
        public static ILiteCollection<WarningBotSchema> WarningCollection()
        {
            return _warningCollection;
        }

        /// <summary>
        /// Returns the warningBoxCurrentState collection
        /// In here, the current warning state of every single member is saved in this table
        /// </summary>
        /// <returns></returns>
        public static ILiteCollection<WarningBotCurrentStatesSchema> WarningCurrentStateCollection()
        {
            return _warningCurrentStateCollection;
        }

        /// <summary>
        /// Returns the warningchangelog collection
        /// In here, the guilds who already received or who didnt receive the changelog message are stored
        /// </summary>
        /// <returns></returns>
        public static ILiteCollection<WarningBotChangeLogSchema> WarningChangeLogCollection()
        {
            return _warningChangeLogCollection;
        }

        /// <summary>
        /// Returns the warningprefix collection
        /// In here, the custom prefix for every guild is saved
        /// </summary>
        /// <returns></returns>
        public static ILiteCollection<WarningBotPrefixSchema> WarningPrefixCollection()
        {
            return _warningPrefixCollection;
        }

        /// <summary>
        /// Returns the warningroles collection
        /// In here, all roles that can access the commands are saved
        /// </summary>
        /// <returns></returns>
        public static ILiteCollection<WarningBotRolesSchema> WarningRolesCollection()
        {
            return _warningRolesCollection;
        }
        
        /// <summary>
        /// Disconnects the database connection and writes back the .log file to the .db file
        /// </summary>
        /// <returns></returns>
        public static void WriteLogToDatabaseFile()
        {
            try
            {
                //close the database connection
                _dataBase.Dispose();
                //null the object
                _dataBase = null;
                //renew the connection
                LoadDatabase();
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogLevel.Critical, "StaticDatabase:WriteLogToDatabaseFile");
            }
        }
    }
}
