namespace SahneeBotModel;

/// <summary>
/// Generates unique Id's for database objects
/// </summary>
public class IdGenerator
{
    private const long TwEpoch = 1643414400000L; // Custom Epoch (Sat, 29 Jan 2022 00:00:00 UTC/GMT+1)
    private const int WorkerIdBits = 4;
    private const long MaxWorkerId = -1L ^ -1L << WorkerIdBits;
    private const int SequenceBits = 10;
    private const int WorkerIdShift = SequenceBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits;
    private const long SequenceMask = -1L ^ -1L << SequenceBits;
    private static readonly DateTime Utc1970 = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private readonly long _workerId;
    private long _sequence;
    private long _lastTimestamp = -1L;
    
    /// <summary>
    /// Creates a new ID generator.
    /// </summary>
    /// <param name="workerId">Id of the current worker.</param>
    public IdGenerator(long workerId)
    {
        if (workerId > MaxWorkerId || workerId < 0)
            throw new Exception($"worker Id can't be greater than {workerId} or less than 0 ");
        _workerId = workerId;
    }

    /// <summary>
    /// Generate the next snowflake ID
    /// </summary>
    /// <returns>The ID.</returns>
    /// <exception cref="Exception">Failed to generate the ID.</exception>
    public long NextId()
    {
        lock (this)
        {
            var timestamp = TimeGen();
            if (_lastTimestamp == timestamp)
            {
                //generated in the same microsecond ID
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                {
                    //Generated in one microsecond ID count reached maximum, waiting for next microsecond
                    timestamp = TillNextMillis(_lastTimestamp);
                }
            }
            else
            {
                //generated in different microseconds ID
                _sequence = 0; //Count clear
            }

            if (timestamp < _lastTimestamp)
            {
                //If the current timestamp is more than the last one generated ID The timestamp is small
                //, throwing an exception because there is no guarantee that it will be generated
                //now ID No previous build
                throw new Exception("Clocked moved backwards! Refusing to generate id for " +
                                    $"{_lastTimestamp - timestamp}");
            }

            _lastTimestamp = timestamp; //Save current timestamp as last generated ID Timestamp
            var nextId = (timestamp - TwEpoch << TimestampLeftShift) 
                         | _workerId << WorkerIdShift | _sequence;
            return nextId;
        }
    }

    /// <summary>
    /// Get the next microsecond timestamp
    /// </summary>
    /// <param name="lastTimestamp">The last timestamp as long.</param>
    /// <returns>The new timestamp as long.</returns>
    private static long TillNextMillis(long lastTimestamp)
    {
        var timestamp = TimeGen();
        while (timestamp <= lastTimestamp)
        {
            timestamp = TimeGen();
        }

        return timestamp;
    }
    
    /// <summary>
    /// Generate the current timestamp
    /// </summary>
    /// <returns>the current timestamp as long</returns>
    private static long TimeGen()
    {
        return (long)(DateTime.UtcNow - Utc1970).TotalMilliseconds;
    }
}
