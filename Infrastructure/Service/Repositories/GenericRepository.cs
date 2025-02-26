using MongoDB.Bson;
using MongoDB.Driver;
using SayHenTai_WebApp.Infrastructure.MongoDB;

namespace SayHenTai_WebApp.Infrastructure.Service.Repositories
{
    public class GenericRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public GenericRepository(MongoDbContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var objectId = new ObjectId(id);
            return await _collection.Find(Builders<T>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(string id, T entity)
        {
            var objectId = new ObjectId(id);
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", objectId), entity);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", objectId));
        }

        public async Task<T> GetByColumnNameAsync(string columnName, string value)
        {
            return await _collection.Find(Builders<T>.Filter.Eq(columnName, value)).FirstOrDefaultAsync();
        }

        public async Task<List<T>> FindAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<T>> InsertManyAsync(List<T> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                throw new ArgumentException("Danh sách không được rỗng.");
            }

            await _collection.InsertManyAsync(entities);
            return entities; // Trả về danh sách đối tượng đã chèn (bao gồm Id)
        }
    }
}
