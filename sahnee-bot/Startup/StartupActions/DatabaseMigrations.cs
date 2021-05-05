using System;
using LiteDB;
using sahnee_bot.Database;
using sahnee_bot.Logging;

namespace sahnee_bot.Startup.StartupActions
{
    public class DatabaseMigrations
    {
        private static readonly Logger _logger = new Logger();
        
        public static void DoDatabaseMigrations()
        {
            Migration_Add_Custom_Prefix_2();
        }

        /// <summary>
        /// Migration for adding the custom prefix2 to the table
        /// </summary>
        private static void Migration_Add_Custom_Prefix_2()
        {
            try
            {
                //migrate if null
                if (StaticDatabase.WarningPrefixCollection().Query().First().CustomPrefix2 == null)
                {
                    var collectionSource = StaticDatabase.GetDatabase().GetCollection("warningbot_prefix");
                    var collectionTemp = StaticDatabase.GetDatabase().GetCollection("warningbot_prefix_temp");
                    var documents = collectionSource.FindAll();
                    foreach (var document in documents)
                    {
                        document["CustomPrefix2"] = new BsonValue(document["CustomPrefix"].AsString);
                        collectionTemp.Insert(document);
                    }

                    StaticDatabase.GetDatabase().DropCollection("warningbot_prefix");
                    StaticDatabase.GetDatabase().Commit();
                    bool test = StaticDatabase.GetDatabase().RenameCollection("warningbot_prefix_temp", "warningbot_prefix");
                    StaticDatabase.GetDatabase().Commit();
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogLevel.Error, "DatabaseMigrations:Migration_Add_Custom_Prefix_2");
            }
        }
    }
}
