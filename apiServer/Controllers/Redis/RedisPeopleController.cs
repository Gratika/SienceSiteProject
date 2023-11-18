using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace apiServer.Controllers.Redis
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisPeopleController : ControllerBase
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisPeopleController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }
        [HttpPost("AddPeople")]
        public void AddPeople (People model)
        {
            var hashKey = $"People:{model.Id}";
            var hashFields = new HashEntry[]
            {
    new HashEntry("Id", model.Id),
    new HashEntry("name", model.name ?? string.Empty),
    new HashEntry("surname", model.surname ?? string.Empty),
    new HashEntry("birthday", model.birthday.ToString() ?? string.Empty),
    new HashEntry("date_created_people", model.date_create.ToString()),
    new HashEntry("date_modified_people", model.modified_date.ToString()),
    new HashEntry("path_bucket", model.path_bucket ?? string.Empty),
            };

            _database.HashSet(hashKey, hashFields);
        }
        [HttpPost("GetPeople")]
        public People GetPeople(string id)
        {
            // Получение хэша из Redis
            HashEntry[] hashFields = _database.HashGetAll($"People:{id}");

            People people = new People();
            foreach (var hashField in hashFields)
            {
                if (hashFields.Length != 0)
                    switch (hashField.Name.ToString())
                    {                       
                        //выгрузка people
                        case "Id":
                            people.Id = hashField.Value;
                            break;
                        case "name":
                            people.name = hashField.Value;
                            break;
                        case "surname":
                            people.surname = hashField.Value;
                            break;
                        case "birthday":
                            people.birthday = DateTime.Parse(hashField.Value);
                            break;
                        case "date_created_people":
                            people.date_create = DateTime.Parse(hashField.Value);
                            break;
                        case "date_modified_people":
                            people.modified_date = DateTime.Parse(hashField.Value);
                            break;
                        case "path_bucket":
                            people.path_bucket = hashField.Value;
                            break;
                    }
            }

            return people;
        }   
    }
}
