using apiServer.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace apiServer.Controllers.Redis
{
    public class RedisEmotionController : Controller
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisEmotionController(string connectionString)
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
