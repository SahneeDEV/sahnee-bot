namespace sahnee_bot.Logging
{
    /// <summary>
    /// Specifies the level and importance of a log message
    /// Counting up from 0 to infinity
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Logs that are relevant and can affect the stability of the program
        /// </summary>
        Critical = 0,
        /// <summary>
        /// Logs that may be relevant but can also occur if for example missing permissions for the bot
        /// </summary>
        Error = 1,
        /// <summary>
        /// Logs that contain anomalies in the procedure of an execution
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Logs that contain some information about the procedure
        /// </summary>
        Info = 3,
        /// <summary>
        /// Logs that are used for more detailed debugging while developing
        /// </summary>
        Verbose = 4,
        /// <summary>
        /// Logs that contain almost every information available
        /// </summary>
        Debug = 5
    }
}
