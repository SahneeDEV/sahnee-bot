using System.Threading.Tasks;

namespace sahnee_bot.OtherAPI
{
    /// <summary>
    /// Represents the basic api class
    /// </summary>
    public interface MainApi
    {
        /// <summary>
        /// Sends something to the botlist api
        /// </summary>
        /// <returns></returns>
        public Task SendStatisticsAsync();
    }
}
