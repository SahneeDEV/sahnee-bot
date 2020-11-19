using System;

namespace sahnee_bot.Util
{
    public class Configuration
    {
        /// <summary>
        /// Logging and Debugging
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Global
        public class ConfigLogging
        {
            // ReSharper disable once ClassNeverInstantiated.Global
            public class ConfigLogLevel
            {
                public string Default { get; set; }
                public string System { get; set; }
                public string Microsoft { get; set; }
            }
            public ConfigLogLevel LogLevel { get; set; }
        }

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
            public string CommandChannel { get; set; }
            public TimeSpan DatabaseCleanup { get; set; }
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
            public string PunishMessage { get; set; }
        }

        //Declare the Subclasses for export
        public ConfigLogging Logging { get; set; }
        public ConfigGeneral General { get; set; }
        public ConfigWarningBot WarningBot { get; set; }
    }
}
