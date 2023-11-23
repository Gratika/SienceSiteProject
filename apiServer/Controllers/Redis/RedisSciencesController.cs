using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Xml.Linq;

namespace apiServer.Controllers.Redis
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisSciencesController : ControllerBase
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisSciencesController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        [HttpPost("AddSciences")]
        public void AddSciences(List<Sciences> model)
        {
            foreach (Sciences science in model)
            {
                var userKey = $"Science:{science.Id}";
                var userFields = new HashEntry[]
                {
                new HashEntry("Id", science.Id),
                new HashEntry("name", science.name),
                new HashEntry("note", science.note ?? string.Empty),
                };
                _database.HashSet(userKey, userFields);
            }
        }
        [HttpPost("GetScience")]
        public Sciences GetScience(string Id)
        {
            // Получение хэша из Redis
            HashEntry[] hashFields = _database.HashGetAll($"Science:{Id}");

            Sciences science = new Sciences();
            foreach (var hashField in hashFields)
            {
                if (hashFields.Length != 0)
                    switch (hashField.Name.ToString())
                    {
                        //выгрузка Sciences
                        case "Id":
                            science.Id = hashField.Value;
                            break;
                        case "name":
                            science.name = hashField.Value;
                            break;
                        case "note":
                            science.note = hashField.Value;
                            break;                    
                    }
            }

            return science;
        }
        [HttpPost("GetAllSciences")]
        public List<Sciences> GetAllSciences()
        {
            List<Sciences> sciences = new List<Sciences>();
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
                if (key == $"Science:{IdForGetArticle}")
                {
                    sciences.Add(GetScience(IdForGetArticle));
                }
            }
            return sciences;
        }

    }
}
