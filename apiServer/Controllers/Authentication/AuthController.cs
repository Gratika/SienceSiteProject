using apiServer.Controllers.Redis;
using apiServer.Models;
using apiServer.Models.ForUser;
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

                List<Users> users = _context.Users.Include(a => a.people_).ToList();

                for (int i = 0; i < 2; i++)
                {
                    Response.user = await CheckUserUnique(users, userRequest.password, userRequest.email);
                    if (Response.user != null)
                    {
                        Response.user.access_token = _tokens.GenerateAccessToken(Response.user.Id);
                        _context.Users.Update(Response.user);
                        _context.SaveChanges();
                        _redisRepository.AddOneModel(Response.user);
                        Response.answer = "Вы вошли";                      
                        return Ok(Response);
                    }
                    users = await _context.Users.Include(a => a.people_).ToListAsync();
                }
                return Ok("Вы не вошли");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("CheckUserUnique")]
        public async Task<Users> CheckUserUnique(List<Users> users, string password, string email)
        {
            foreach (Users user in users)
            {
                if (string.Equals(user.password, password, StringComparison.Ordinal) == true && string.Equals(user.email, email, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
