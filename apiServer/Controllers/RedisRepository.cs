using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace apiServer.Controllers
{
    public class RedisRepository : Controller
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisRepository(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        public void AddEmotion(int emotionId, string name, string Emoji)
        {
            var userKey = $"emotions:{emotionId}";
            var userFields = new HashEntry[]
            {
            new HashEntry("Name", name),
            new HashEntry("Value", Emoji),
            //new HashEntry("CreatedAt", DateTime.UtcNow.ToString()),
            };

            _database.HashSet(userKey, userFields);
        }
        public HashEntry[] GetEmotion(string key)
        {
            var db = _redis.GetDatabase();
            var hashEntries = db.HashGetAll(key);
            return hashEntries;
        }
    }
}
