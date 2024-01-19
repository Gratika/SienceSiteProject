using apiServer.Controllers.ForModels;
using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Policy;

namespace apiServer.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase // Регистрация
    {
        private readonly ArhivistDbContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly RedisController _redisRepository;
        private readonly TokensController _tokens;
        private readonly EmailController _em;
        private readonly PeopleController _people;
        public UserController(ArhivistDbContext context, ILogger<UserController> logger, TokensController tokens, IHttpClientFactory httpClientFactory, EmailController em, PeopleController people,EmailController emailController)
        {
            _context = context;
            _logger = logger;
            _tokens = tokens;
            _redisRepository = new RedisController("redis:6379,abortConnect=false");
            _people = people;
            _em = emailController;
        }
        [HttpGet("IsUserUnique")]
        public async Task<int> IsUserUnique(Users Person/* string password, string email*/)
        {
            try
            {
                List<Users> users = _redisRepository.GetAllData<Users>();
                // проверка данных в редис  
                for ( int i = 0;i < 2; i++)
                {
                    if (CheckPasswordAndEmail(users, Person.password,Person.email) == false)
                    {
                        return 0;
                    }
                    users = await _context.Users.ToListAsync();
                }
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(string password, string email /*UserRequest userRequest*/) //Регистрация
        {
            try
            {
                People peopleOn = _people.CreatePeople();
            peopleOn.Id = Guid.NewGuid().ToString();
            peopleOn.name = /*userRequest.people.name*/ "sadas";
            peopleOn.surname = /*userRequest.people.surname*/ "dasdas";
            peopleOn.birthday = /*userRequest.people.birthday*/ DateTime.Now;

                Users FirstEx = new Users();
                FirstEx.Id = Guid.NewGuid().ToString();
                FirstEx.login = /*userRequest.*/email;
                FirstEx.password = /*userRequest.*/password;
                FirstEx.email = /*userRequest.*/email;
                FirstEx.date_create = DateTime.Now;
                FirstEx.modified_date = DateTime.Now;
                FirstEx.role_id = "1";

                FirstEx.people_id = peopleOn.Id;
                //Проверка уникальности пользователя по паролю и email
                if (await IsUserUnique(FirstEx) == 0)
                {
                    return Ok("Пользователь с таким логином, паролем или email уже существует.");
                }
                else
                {
                    var response = await _em.SentCode(/*userRequest.*/email); //Отправляем письмо пользователю для проверки почты
                    if (response == "1")
                    {
                    //// Почта прошла проверку, продолжаем регистрацию
                    FirstEx.access_token = _tokens.GenerateAccessToken(FirstEx.Id);
                    //FirstEx.refresh_token = _tokens.GenerateRefreshToken(FirstEx.Id);
                    //// Сохранение пользователя в базе данных
                    _people.AddPeopleToDb(peopleOn);
                        _context.Users.Add(FirstEx);
                        _context.SaveChanges();
                        ////Добавляем в редис               
                        _redisRepository.AddOneModel(FirstEx);
                    ////генерируем токены                  
                    return Ok(new { Message = "Регистрация прошла успешно" });
                    }
                    else
                    {
                        // Почта не прошла проверку, возвращаем ошибку
                        return BadRequest(new { Error = "Ошибка при проверке почты" });
                    }

                    //return Ok("Вы успешно зарегистрировались");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("CheckPasswordAndEmail")]
        public bool CheckPasswordAndEmail(List<Users> users,string password, string email)
        {
            foreach (Users user in users)
            {
                if (string.Equals(user.password, password, StringComparison.Ordinal) == true || string.Equals(user.email, email, StringComparison.Ordinal) == true)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
