using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly RedisPeopleController _redisPeopleController;
        public PeopleController(ArhivistDbContext context)
        {
            _context = context;
            _redisPeopleController = new RedisPeopleController("redis:6379,abortConnect=false");
        }
        [HttpPost("AddPeopleToDb")]
        public string AddPeopleToDb(People people)
        {
            if(string.Equals(people.name, "") || string.Equals(people.surname, ""))              
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
        [HttpPost("CreatePeople")]
        public People CreatePeople()
        {
            People people = new People();
            people.Id = Guid.NewGuid().ToString();
            people.birthday = DateTime.Now;
            people.date_create = DateTime.Now;
            people.modified_date = DateTime.Now;
            _redisPeopleController.AddPeople(people);
            return people;
        }
        [HttpPost("GetPeopleFromRedis")]
        public People GetPeopleFromRedis(string id)
        {
            People people = _redisPeopleController.GetPeople(id);
            
            return people;
        }
    }
}
