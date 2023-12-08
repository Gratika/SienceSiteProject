using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;

namespace apiServer.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase // вход в аккаунт
    {
        private readonly ArhivistDbContext _context;
        private readonly RedisController _redisRepository;
        private readonly TokensController _tokens;

        public AuthController(ArhivistDbContext context, TokensController tokens)
        {
            _context = context;
            _tokens = tokens;
            _redisRepository = new RedisController("redis:6379,abortConnect=false");
        }

        [HttpPost("AuthUser")]
        public async Task<ActionResult> AuthUser(/*string pas, string email*/UserRequest userRequest) //авторизация
        {
            AuthResponse Response = new AuthResponse();
            try
            {
                //UserRequest userRequest = new UserRequest();
                //userRequest.email = email;
                //userRequest.password = pas;           

                List<Users> users = _redisRepository.GetAllData<Users>();
                // проверка данных в редис  
                for (int i = 0; i < 2; i++)
                {
                    Response.user = await CheckUserUnique(users, userRequest.password, userRequest.email);
                    if (Response.user != null)
                    {
                        _redisRepository.AddOneModel(Response.user);
                        Response.answer = "Вы вошли";                      
                        return Ok(Response);
                    }
                    users = await _context.Users.Include(a => a.people_).ToListAsync();
                }
                return Ok("Вы не вошли");
            }
            catch
            {
                Response.answer = "Вы не вошли";
                return Ok(new { Message = Response });
            }
        }
        [HttpGet("CheckUserUnique")]
        public async Task<Users> CheckUserUnique(List<Users> users, string password, string email)
        {
            foreach (Users user in users)
            {
                if (string.Equals(user.password, password, StringComparison.Ordinal) == true && string.Equals(user.email, email, StringComparison.Ordinal) == true)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
