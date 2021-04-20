using System;

namespace sahnee_bot.Configuration
{
    public class Configuration
    {
        /// <summary>
        /// General Configuration
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Global
        public class ConfigGeneral
        {
            public string Id { get; set; }
            public int Permissions { get; set; }
            public string Token { get; set; }
            public char CommandPrefix { get; set; }
            public string DatabasePath { get; set; }
            public string ChangeLogPath { get; set; }
            public string CommandChannel { get; set; }
            public TimeSpan DatabaseCleanup { get; set; }
            public int LogLevel { get; set; }
        }

        /// <summary>
        /// Warning Bot
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Global
        public class ConfigWarningBot
        {
            public string WarningPrefix { get; set; }
            public TimeSpan WarningRoleCleanup { get; set; }
            public string DatabaseCollection { get; set; }
            public uint WarningHistoryCount { get; set; }
            public uint WarningLeaderboardCount { get; set; }
            public string[] Admins { get; set; }
            public string[] Mods { get; set; }
            public string PunishMessage { get; set; }
        }

        /// <summary>
        /// For all external APIs
        /// </summary>
        //ReSharper disable once ClassNeverInstantiated.Global
        public class ConfigExternalApi
        {
            //ReSharper disable once ClassNeverInstantiated.Global
            public class ConfigDiscordBotList
            {
                public string ApiUrl { get; set; }
                public string BotId { get; set; }
                public string AuthToken { get; set; }
            }

            public ConfigDiscordBotList DiscordBotList { get; set; }
        }

        //Declare the Subclasses for export
        public ConfigGeneral General { get; set; }
        public ConfigWarningBot WarningBot { get; set; }
        public ConfigExternalApi ExternalApi { get; set; }
    }
}
