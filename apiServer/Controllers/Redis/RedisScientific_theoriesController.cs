using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace apiServer.Controllers.Redis
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisScientific_theoriesController : ControllerBase
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisScientific_theoriesController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        [HttpPost("AddScientific_theories")]
        public void AddScientific_theories(List<Scientific_theories> model)
        {
            foreach (Scientific_theories science in model)
            {
                var userKey = $"Scientific_theories:{science.Id}";
                var userFields = new HashEntry[]
                {
                new HashEntry("Id", science.Id),
                 new HashEntry("science_id", science.science_id),
                new HashEntry("name", science.name),
                new HashEntry("note", science.note ?? string.Empty),
                };
                _database.HashSet(userKey, userFields);
            }
        }
        [HttpPost("GetScientific_theorie")]
        public Scientific_theories GetScientific_theories(string Id)
        {
            // Получение хэша из Redis
            HashEntry[] hashFields = _database.HashGetAll($"Scientific_theories:{Id}");

            Scientific_theories science = new Scientific_theories();
            foreach (var hashField in hashFields)
            {
                if (hashFields.Length != 0)
                    switch (hashField.Name.ToString())
                    {
                        //выгрузка Sciences
                        case "Id":
                            science.Id = hashField.Value;
                            break;
                        case "science_id":
                            science.science_id = hashField.Value;
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
        [HttpPost("GetAllScientific_theories")]
        public List<Scientific_theories> GetAllScientific_theories()
        {
            List<Scientific_theories> sciences = new List<Scientific_theories>();
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
                Scientific_theories scientific = new Scientific_theories();
                if(key == $"Scientific_theories:{IdForGetArticle}")
                {
                    sciences.Add(GetScientific_theories(IdForGetArticle));
                }                
            }
            return sciences;
        }
    }
}
