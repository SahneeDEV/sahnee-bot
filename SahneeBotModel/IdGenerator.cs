namespace SahneeBotModel;

/// <summary>
/// Generates unique Id's for database objects
/// </summary>
public class IdGenerator
{
    //Variables
    private static long workerId;
    private static long twepoch = 1643414400000L; // Custom Epoch (Sat, 29 Jan 2022 00:00:00 UTC/GMT+1)
    private static long sequence = 0L;
    private static int workerIdBits = 4;
    public static long maxWorkerId = -1L ^ -1L << workerIdBits;
    private static int sequenceBits = 10;
    private static int workerIdShift = sequenceBits;
    private static int timestampLeftShift = sequenceBits + workerIdBits;
    public static long sequenceMask = -1L ^ -1L << sequenceBits;
    private long lastTimestamp = -1L;


    /// <summary>
    /// Constructor
    /// <param name="workerId">Id of the current worker. Gathered from appsettings.json</param>
    /// </summary>
    public IdGenerator(long workerId)
    {
        if (workerId > maxWorkerId || workerId < 0)
            throw new Exception($"worker Id can't be greater than {workerId} or less than 0 ");
        IdGenerator.workerId = workerId;
    }

    /// <summary>
    /// Generate the next snowflake ID
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public long NextId()
    {
        lock (this)
        {
            long timestamp = timeGen();
            if (this.lastTimestamp == timestamp)
            {
                //generated in the same microsecond ID
                IdGenerator.sequence = (IdGenerator.sequence + 1) & IdGenerator.sequenceMask;
                if (IdGenerator.sequence == 0)
                {
                    //Generated in one microsecond ID count reached maximum, waiting for next microsecond
                    timestamp = tillNextMillis(this.lastTimestamp);
                }
            }
            else
            {
                //generated in different microseconds ID
                IdGenerator.sequence = 0; //Count clear
            }

            if (timestamp < lastTimestamp)
            {
                //If the current timestamp is more than the last one generated ID The timestamp is small
                //, throwing an exception because there is no guarantee that it will be generated
                //now ID No previous build
                throw new Exception("Clocked moved backwards! Refusing to generate id for " +
                                    $"{ this.lastTimestamp - timestamp}");
            }

            this.lastTimestamp = timestamp; //Save current timestamp as last generated ID Timestamp
            long nextId = (timestamp - twepoch << timestampLeftShift) 
                          | IdGenerator.workerId << IdGenerator.workerIdShift | IdGenerator.sequence;
            return nextId;
        }
    }

    /// <summary>
    /// Get the next microsecond timestamp
    /// </summary>
    /// <param name="lastTimestamp"></param>
    /// <returns></returns>
    private long tillNextMillis(long lastTimestamp)
    {
        long timestamp = timeGen();
        while (timestamp <= lastTimestamp)
        {
            timestamp = timeGen();
        }

        return timestamp;
    }
    
    /// <summary>
    /// Generate the current timestamp
    /// </summary>
    /// <returns>the current timestamp as long</returns>
    private long timeGen()
    {
        return (long)(DateTime.UtcNow - 
                      new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .TotalMilliseconds;
    }
}
