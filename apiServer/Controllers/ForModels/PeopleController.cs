using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace apiServer.Controllers.ForModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly RedisController _redisController;
        public PeopleController(ArhivistDbContext context)
        {
            _context = context;
            _redisController = new RedisController("redis:6379,abortConnect=false");
        }
        [HttpPost("AddPeopleToDb")]
        public string AddPeopleToDb(People people)
        {
            try
            {
                if (string.Equals(people.name, "") || string.Equals(people.surname, ""))
                {
                    _context.people.Update(people);
                    _context.SaveChanges();
                    return "Update " + people.name + "  " + people.surname;
                }
                else
                {
                    _context.people.Add(people);
                    _context.SaveChanges();
                    return "Add";
                }
            }
            catch
            {
                throw new Exception();
            }

        }
        [HttpPost("CreatePeople")]
        public People CreatePeople()
        {
            try
            {
                People people = new People();
                people.Id = Guid.NewGuid().ToString();
                people.birthday = DateTime.Now;
                people.date_create = DateTime.Now;
                people.modified_date = DateTime.Now;
                _redisController.AddOneModel(people);
                return people;
            }
            catch
            {
                throw new Exception();
            }            
        }
        [HttpGet("GetPeopleFromRedis")]
        public People GetPeopleFromRedis(string id)
        {
            People people = _redisController.GetData<People>(id);

            return people;
        }
    }
}
