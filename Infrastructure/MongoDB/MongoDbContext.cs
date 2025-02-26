using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace SayHenTai_WebApp.Infrastructure.MongoDB
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IOptions<MongoOptions> _options;
        public MongoDbContext(IOptions<MongoOptions> options)
        {
            _options = options;
            var client = new MongoClient(_options.Value.ConnectionString);
            _database = client.GetDatabase(_options.Value.DatabaseName); // Thay thế bằng tên database của bạn
        }

        /// <summary>
        /// GetCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName">là tên table</param>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
