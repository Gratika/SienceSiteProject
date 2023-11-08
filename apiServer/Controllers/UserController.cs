using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Policy;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase // Регистрация
    {
        private readonly ArhivistDbContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly RedisUserController _redisRepository;
        private readonly TokensController _tokens;
        private readonly EmailController _em;
        public UserController(ArhivistDbContext context, ILogger<UserController> logger, TokensController tokens, IHttpClientFactory httpClientFactory, EmailController em)
        {
            _context = context;
            _logger = logger;
            _tokens = tokens;
            _redisRepository = new RedisUserController("redis:6379,abortConnect=false");
            _em = em;
        }
        [HttpGet("IsUserUnique")]
        public async Task<int> IsUserUnique(/*Users Person*/ string password, string email)
        {
            // проверка данных в редис  
            if (_redisRepository.IsUserUnique(/*Person.password, Person.email*/ password, email) == true)
            {
                return 1;
            }             
            List<Users> users = await _context.Users.ToListAsync();
            foreach (Users user in users)
            {
                if (string.Equals(/*user.password, Person.password*/user.password, password, StringComparison.Ordinal) == true || string.Equals(/*user.email, Person.email,*/user.email, email, StringComparison.Ordinal) == true)
                {
                    return 0;
                }
            }
            return 1;
        }
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(/*string pas, string email*/UserRequest userRequest) //Регистрация
        {
            //userRequest.email = "sacas";
            //userRequest.password = "ascasca";
            Users FirstEx = new Users();
            FirstEx.login = userRequest.email;
            FirstEx.password =userRequest.password;
            FirstEx.email = userRequest.email;
            FirstEx.date_create = DateTime.Now;
            FirstEx.modified_date = DateTime.Now;
            FirstEx.role_id = 1;
            //Проверка уникальности пользователя по паролю и email
            if (await IsUserUnique(/*FirstEx */FirstEx.password, FirstEx.email) == 0)
            {
                return Ok("Пользователь с таким логином, паролем или email уже существует.");
            }
            else
            {
                var response = await _em.SentCode(userRequest.email); //Отправляем письмо пользователю для проверки почты
                // Обработка ответа от микросервиса для проверки почты
                if (response == "1")
                {
                    //// Почта прошла проверку, продолжаем регистрацию
                    ////генерируем токены
                    FirstEx.access_token = _tokens.GenerateAccessToken();
                    FirstEx.refresh_token = _tokens.GenerateRefreshToken();
                    //// Сохранение пользователя в базе данных
                    _context.Users.Add(FirstEx);
                    _context.SaveChanges();
                    ////Добавляем в редис               
                    _redisRepository.AddUser(FirstEx);                    
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

        [HttpGet("CheckTokens")]
        public string CheckTokens(string accessToken,string refreshToken)
        {            
            Users user = new Users();
            user = _redisRepository.GetUsersRedis(refreshToken);// Проверка наличия данных в кэше
            if (user.email == "0") // Данные отсутствуют в кэше, выполняем запрос к базе данных
            {
                user = _context.Users.FirstOrDefault(u => u.access_token == accessToken);             
            }
            // Поиск пользователя в базе данных по AccessToken пользователя
            if (_tokens.IsTokenExpired(accessToken) && user != null)
                {
                    if (_tokens.IsTokenExpired(refreshToken))
                    {
                        return "Введите заново емаил и пароль";
                    }
                    else
                    {
                        _context.Users.Remove(user);
                        _redisRepository.DeleteKey("users:" + refreshToken);
                        user.access_token = accessToken = _tokens.GenerateAccessToken();
                        user.refresh_token = refreshToken = _tokens.GenerateRefreshToken();
                        _context.Users.Update(user);
                        _context.SaveChanges();             
                    // Сохранение/обновление данных в кэше на 10 минут                  
                    _redisRepository.AddUser(user);
                    return "Токены обновлены";
                    }
                }
                else if(user != null)
                {               
                    return "Токен НЕ нуждается в обновлении(Пользователю разрешают войти на страницу)";
                }
            else
            {
                return "Пользователь не найден";
            }
        }
        
    }
}
