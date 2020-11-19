using System.Threading.Tasks;

namespace sahnee_bot.Util
{
    public static class StaticLock
    {
        //Variables

        /// <summary>
        /// Command warn
        /// </summary>
        private static bool _lockCommandWarning = false;
        private static readonly object _lockObjectCommandWarning = new object();

        /// <summary>
        /// Command warnhistory
        /// </summary>
        private static bool _lockCommandWarningHistroy = false;
        private static readonly object _lockObjectCommandWarninghistory = new object();

        /// <summary>
        /// Command warningstoday
        /// </summary>
        private static bool _lockCommandWarningsToday = false;
        private static readonly object _lockObjectCommandWarnignsToday = new object();
        
        /// <summary>
        /// All
        /// </summary>
        private static readonly object _lockObjectAll = new object();

        #region Getter

        /// <summary>
        /// Tries to lock the warn command
        /// </summary>
        /// <returns></returns>
        private static bool TryLockCommandWarning()
        {
            lock (_lockObjectCommandWarning)
            {
                if (_lockCommandWarning == false)
                {
                    _lockCommandWarning = true;
                    return _lockCommandWarning;
                }
                return false;                
            }
        }

        /// <summary>
        /// Tries to lock the warnhistroy command
        /// </summary>
        /// <returns></returns>
        private static bool TryLockCommandWarningHistory()
        {
            lock (_lockObjectCommandWarninghistory)
            {
                if (_lockCommandWarningHistroy == false)
                {
                    _lockCommandWarningHistroy = true;
                    return _lockCommandWarningHistroy;
                }
                return false;
            }
        }
        
        /// <summary>
        /// Tries to lock the warningstoday command
        /// </summary>
        /// <returns></returns>
        private static bool TryLockCommandWarningsToday()
        {
            lock (_lockObjectCommandWarnignsToday)
            {
                if (_lockCommandWarningsToday == false)
                {
                    _lockCommandWarningsToday = true;
                    return _lockCommandWarningsToday;
                }
                return false;
            }
        }

        #endregion

        #region Setter

        /// <summary>
        /// Unlocks the warn command
        /// </summary>
        /// <returns></returns>
        public static bool UnlockCommandWarning()
        {
            lock (_lockObjectCommandWarning)
            {
                if (_lockCommandWarning)
                {
                    _lockCommandWarning = false;
                    return true;
                }
                if (_lockCommandWarning == false)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Unlocks the warnhistory command
        /// </summary>
        /// <returns></returns>
        public static bool UnlockCommandWarnHistory()
        {
            lock (_lockObjectCommandWarninghistory)
            {
                if (_lockCommandWarningHistroy)
                {
                    _lockCommandWarningHistroy = false;
                    return true;
                }
                if (_lockCommandWarning == false)
                {
                    return true;
                }
                return false;
            }
        }
        
        /// <summary>
        /// Unlocks the warningstoday command
        /// </summary>
        /// <returns></returns>
        public static bool UnlockCommandWarningsToday()
        {
            lock (_lockObjectCommandWarnignsToday)
            {
                if (_lockCommandWarningsToday)
                {
                    _lockCommandWarningsToday = false;
                    return true;
                }
                if (_lockCommandWarningsToday == false)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Unlocks all commands
        /// </summary>
        /// <returns></returns>
        public static bool UnlockAll()
        {
            lock (_lockObjectAll)
            {
                if (_lockCommandWarningsToday)
                {
                    _lockCommandWarningsToday = false;
                }
                if (_lockCommandWarning)
                {
                    _lockCommandWarning = false;
                }
                if (_lockCommandWarningHistroy)
                {
                    _lockCommandWarningHistroy = false;
                }
                if (!_lockCommandWarning && !_lockCommandWarningHistroy && !_lockCommandWarningsToday)
                {
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region LockAquiration

        /// <summary>
        /// Locks the warn command if possible
        /// otherwise it will wait
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static async Task AquireWarningAsync(int delay = 100)
        {
            while (!TryLockCommandWarning())
            {
                await Task.Delay(delay);
            }
        }

        /// <summary>
        /// Locks the warnhistory command if possible
        /// otherwise it will wait
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static async Task AquireWarningHistroyAsync(int delay = 100)
        {
            while (!TryLockCommandWarningHistory())
            {
                await Task.Delay(delay);
            }
        }
        
        /// <summary>
        /// Locks the warningstoday command if possible
        /// otherwise it will wait
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static async Task AquireWarningsTodayAsync(int delay = 100)
        {
            while (!TryLockCommandWarningsToday())
            {
                await Task.Delay(delay);
            }
        }

        /// <summary>
        /// Locks every command
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static async Task AquireAllAsync(int delay = 100)
        {
            while (!TryLockCommandWarning() && !TryLockCommandWarningHistory() && !TryLockCommandWarningsToday())
            {
                await Task.Delay(delay);
            }
        }
        
        #endregion
    }
}
