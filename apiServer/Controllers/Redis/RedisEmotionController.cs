using apiServer.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Text;

namespace apiServer.Controllers.Redis
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisEmotionController : Controller
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisEmotionController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }
        [HttpPost("AddEmotion")]
        public void AddEmotion(List<Emotions> emotions)
        {
            foreach (Emotions emotion in emotions)
            {
                var userKey = $"Emotions:{emotion.Id}";
                var userFields = new HashEntry[]
                {
                  new HashEntry("Id", emotion.Id),
                  new HashEntry("Name", emotion.Name),
                  new HashEntry("Emoji", emotion.Emoji),
                };

                _database.HashSet(userKey, userFields);
            }
        }
        [HttpPost("GetEmotion")]
        public Emotions GetEmotion(string id)
        {
            // Получение хэша из Redis
            HashEntry[] hashFields = _database.HashGetAll($"Emotions:{id}");

            Emotions emotion = new Emotions();
            foreach (var hashField in hashFields)
            {
                if (hashFields.Length != 0)
                    switch (hashField.Name.ToString())
                    {
                        //выгрузка people
                        case "Id":
                            emotion.Id = hashField.Value;
                            break;
                        case "Name":
                            emotion.Name = hashField.Value;
                            break;
                        case "Emoji":
                            emotion.Emoji = hashField.Value;
                            break;
                    }
            }
            return emotion;
        }
        [HttpPost("GetAllEmotions")]
        public List<Emotions> GetAllEmotions()
        {
            List<Emotions> emotions = new List<Emotions>();
            var keys = _redis.GetServer("redis", 6379).Keys();

            // Итерация по всем ключам и получение данных
            foreach (var key in keys)
            {
                string IdForGetArticle = "";
                var userFields = _database.HashGetAll(key);
                // Проверка данных на совпадение              
                foreach (var hashEntry in userFields)
                {
                    if (string.Equals(hashEntry.Name.ToString(), "Id"))
                    {
                        IdForGetArticle = hashEntry.Value;
                    }
                }
                if (key == $"Emotions:{IdForGetArticle}")
                {
                    emotions.Add(GetEmotion(IdForGetArticle));
                }
            }
            return emotions;
        }
    }
}
