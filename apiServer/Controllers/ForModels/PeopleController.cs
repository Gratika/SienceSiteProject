﻿using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Authorization;
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("GetPeople")]
        public People GetPeople(string id)
        {
            People people = _redisController.GetData<People>(id);
            if(people == null)
            {
                people = _context.people.FirstOrDefault(a => a.Id == id);
            }

            return people;
        }
       
        [Authorize]
        [HttpPost("RedactPeople")]
        public ActionResult RedactPeople(People people)
        {
            try
            {
                _context.people.Update(people);
                _context.SaveChanges();

                return Ok("Данные пользователя удачно обновленны");
            }
            catch (Exception ex)
            {
                throw ex;
            }        
        }
    }
}
