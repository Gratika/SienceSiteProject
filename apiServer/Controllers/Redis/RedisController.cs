using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;
using System.Reflection;
using System.Xml.Linq;

namespace apiServer.Controllers.Redis
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        [HttpPost("AddOneModel")]
        public void AddOneModel<T>(T data)
        {
                string serializedData = JsonConvert.SerializeObject(data); // Сериализация объекта в JSON
                string key = typeof(T).Name + ":" + GetKeyValue(data);
                _database.StringSet(key, serializedData);
        }
        [HttpPost("AddData")]
        public void AddData<T>(List<T> data)
        {
            foreach (T item in data)
            {
                string serializedData = JsonConvert.SerializeObject(item); // Сериализация объекта в JSON
                string key = typeof(T).Name + ":" + GetKeyValue(item);
                _database.StringSet(key, serializedData);
            }
        }
        [HttpPost("GetData")]
        public T GetData<T>(string key)
        {
            key = $"{typeof(T).Name}:{key}";
            string serializedData = _database.StringGet(key);
            if (serializedData == null)
            {
                return default(T);
            }         
            return JsonConvert.DeserializeObject<T>(serializedData); // Десериализация данных из JSON
        }

        [HttpPost("GetAllData")]
        public List<T> GetAllData<T>()
        {
            EndPoint[] endpoints = _redis.GetEndPoints();
            List<T> allData = new List<T>();

            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                var keys = server.Keys();

                foreach (var key in keys)
                {
                    string serializedData = _database.StringGet(key);
                    T deserializedData = JsonConvert.DeserializeObject<T>(serializedData);
                    allData.Add(deserializedData);
                }
            }
            return allData;
        }
        private string GetKeyValue<T>(T item)
        {
            PropertyInfo propertyInfo = item.GetType().GetProperty("Id");
            if (propertyInfo != null)
            {
                object value = propertyInfo.GetValue(item);
                return value.ToString();
            }
            else
            {
                throw new InvalidOperationException("Не найден'Id'");
         
            }
        }
        [HttpPost("DeleteData")]
        public void DeleteData(string key)
        {
            bool result = _database.KeyDelete(key);
        }
    }
}
