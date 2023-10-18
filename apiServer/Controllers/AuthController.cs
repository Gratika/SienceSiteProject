using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase // вход в аккаунт
    {
        private readonly ArhivistDbContext _context;
        private readonly RedisAuthController _redisRepository;

        public AuthController(ArhivistDbContext context)
        {
            _context = context;
            _redisRepository = new RedisAuthController("redis:6379,abortConnect=false");
        }

        [HttpGet("AuthUser")]
        public async Task<String> AuthUser(string pas, string email/*UserRequest userRequest*/) //авторизация
        {
            // проверка данных в редис  
            if (_redisRepository.IsUserUnique(pas, email /*password, email*/) == true)
            {
                return "Вы вошли";
            }
            if (await CheckUserDatabase(pas, email) == true)
            {
               return "Вы вошли";
            }
            return "Вы не вошли";
        }
        [HttpGet("CheckUserDatabase")]
        public async Task<bool> CheckUserDatabase(string pas, string email /*Users Person*/ )
        {            
            List<Users> users = await _context.Users.ToListAsync();
            foreach (Users user in users)
            {
                if (string.Equals(user.password, pas /*Person.password*/, StringComparison.Ordinal) == true && string.Equals(user.email, email, /*Person.email,*/ StringComparison.Ordinal) == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
