
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using sahnee_bot.Database.Schema;
using sahnee_bot.Util;

namespace sahnee_bot.Database
{
    public static class StaticDatabase
    {
        //Variables
        private static LiteDatabase _dataBase;
        private static readonly Logging _logging = new Logging();

        /// <summary>
        /// Collections
        /// </summary>
        private static ILiteCollection<WarningBotSchema> _warningCollection;
        private static ILiteCollection<WarningBotCurrentStatesSchema> _warningCurrentStateCollection;
        private static ILiteCollection<WarningBotChangeLogSchema> _warningChangeLogCollection;
        private static ulong _warningCollectionId;
        private static ulong _warningCollectionCurrentStateId;
        private static ulong _warningCollectionChangeLogId;

        /// <summary>
        /// Loads a instance of the database
        /// </summary>
        public static void LoadDatabase()
        {
            //get the database or create if it doesn't exist
            _dataBase = new LiteDatabase(StaticConfiguration.GetConfiguration().General.DatabasePath);
            //get or create all necessary collections
            //warning table
            _warningCollection = _dataBase.GetCollection<WarningBotSchema>(StaticConfiguration.GetConfiguration().WarningBot.DatabaseCollection);
            _warningCollectionId = (ulong)_warningCollection.Query().Count() + 1;
            //warning state table
            _warningCurrentStateCollection = _dataBase.GetCollection<WarningBotCurrentStatesSchema>(StaticConfiguration.GetConfiguration().WarningBot.DatabaseCollection + "_state");
            _warningCollectionCurrentStateId = (ulong) _warningCurrentStateCollection.Query().Count() + 1;
            //change log state table
            _warningChangeLogCollection = _dataBase.GetCollection<WarningBotChangeLogSchema>(StaticConfiguration.GetConfiguration().WarningBot.DatabaseCollection + "_changelog");
            _warningCollectionChangeLogId = (ulong) _warningChangeLogCollection.Query().Count() + 1;
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
        /// Increments the id by one
        /// </summary>
        public static void UpdateWarningCollectionId()
        {
            _warningCollectionId = _warningCollectionId + 1;
        }

        /// <summary>
        /// Increments the id by one
        /// </summary>
        public static void UpdateWarningCurrentStateId()
        {
            _warningCollectionCurrentStateId = _warningCollectionCurrentStateId + 1;
        }

        /// <summary>
        /// Increments the id by one
        /// </summary>
        public static void UpdateWarningChangeLogId()
        {
            _warningCollectionChangeLogId = _warningCollectionChangeLogId + 1;
        }

        /// <summary>
        /// Returns the current id
        /// </summary>
        /// <returns></returns>
        public static ulong GetWarningCollectionId()
        {
            return _warningCollectionId;
        }

        /// <summary>
        /// Returns the current id
        /// </summary>
        /// <returns></returns>
        public static ulong GetWarningCurrentStateId()
        {
            return _warningCollectionCurrentStateId;
        }

        /// <summary>
        /// Returns the current id
        /// </summary>
        /// <returns></returns>
        public static ulong GetWarningChangeLogId()
        {
            return _warningCollectionChangeLogId;
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
                _logging.LogToConsoleBase(e.Message);
            }
        }
    }
}
