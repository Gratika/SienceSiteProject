using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace apiServer.Controllers.Redis
{
    public class RedisEmailController : Controller
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisEmailController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }
        public void AddEmail(string email, string code)
        {
            var userKey = $"Email:{code}";
            var userFields = new HashEntry[]
            {
            new HashEntry("Value", email),
            };
            _database.HashSet(userKey, userFields);
        }
        public HashEntry[] GetEmail(string key)
        {
            var db = _redis.GetDatabase();
            var hashEntries = db.HashGetAll(key);
            return hashEntries;
        }
    }
}
