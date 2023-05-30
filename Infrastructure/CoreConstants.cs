namespace SayHenTai_WebApp.Infrastructure
{
    public static class CoreConstants
    {
        /// <summary>
        /// Mặc định là 1 tiếng
        /// </summary>
        public static readonly int DefaultCacheTime = 3600;

        /// <summary>
        /// Mặc định là 1 ngày
        /// Vì sao nên để 1 ngày thay vì 10 ngày như trước do nếu số lượng bản ghi cache
        /// với vào khoảng 10k thì SCAN sẽ khá chậm StackExchange.Redis.RedisTimeoutException: Timeout performing SCAN (5000ms)
        /// </summary>
        public static readonly int DefaultCacheTimeDay = 86400;
    }
}
