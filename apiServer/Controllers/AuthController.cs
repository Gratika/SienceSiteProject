using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase // вход в аккаунт
    {
        private readonly ArhivistDbContext _context;
        private readonly RedisAuthController _redisRepository;
        private readonly TokensController _tokens;

        public AuthController(ArhivistDbContext context, TokensController tokens)
        {
            _context = context;
            _tokens = tokens;
            _redisRepository = new RedisAuthController("redis:6379,abortConnect=false");
        }

        [HttpGet("AuthUser")]
        public async Task<ActionResult> AuthUser(/*string pas, string email*/UserRequest userRequest) //авторизация
        {
            AuthResponse Response = new AuthResponse();
            try
            {               
                Response.user = _redisRepository.IsUserUnique(userRequest.password, userRequest.email );
                // проверка данных в редис  
                if (Response.user != null)
                {
                    Response.answer = "Вы вошли";
                    return Ok(new { Message = Response });
                }
                // проверка данных в базе данных
                Response.user = await CheckUserDatabase(userRequest);
                if (Response.user != null)
                {
                    Response.answer = "Вы вошли";
                    return Ok(new { Message = Response });
                }
            }
            catch
            {
                Response.answer = "Вы не вошли";
                return Ok(new { Message = Response });
            }
            Response.answer = "Вы не вошли";
            return Ok(new { Message = Response });
        }
        [HttpGet("CheckUserDatabase")]
        public async Task<Users> CheckUserDatabase(/*string pas, string email*/ UserRequest Person)
        {            
            List<Users> users = await _context.Users.ToListAsync();
            foreach (Users user in users)
            {
                if (string.Equals(user.password, Person.password, StringComparison.Ordinal) == true && string.Equals(user.email,Person.email, StringComparison.Ordinal) == true)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
