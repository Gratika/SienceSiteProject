using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace apiServer.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : Controller
    {
        private ArhivistDbContext _context;
        private readonly RedisController _redisRepository;
        public readonly IConfiguration _configuration; // необходим для доступа к jwt ключу
        public TokensController(IConfiguration configuration, ArhivistDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _redisRepository = new RedisController("redis:6379,abortConnect=false");
        }

        [HttpGet("GenerateAccessToken")]
        public string GenerateAccessToken(string id)
        {
            try
            {
                var identity = GetIdentity(id);
                if (identity == null)
                {
                    throw new Exception();
                }

                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        notBefore: now,
                        claims: identity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(15)), // Срок действия токена в минутах (15)
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"])), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return encodedJwt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("GenerateRefreshToken")]
        public string GenerateRefreshToken(string id)
        {
            try
            {
                var identity = GetIdentity(id);

                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        notBefore: now,
                        claims: identity.Claims,
                        expires: now.Add(TimeSpan.FromDays(15)), // Срок действия токена в минутах (15)
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"])), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return encodedJwt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("IsTokenExpired")]
        // Метод для проверки срока действия Token
        public bool IsTokenExpired(string accessToken)
        {
            // В данном примере считаем, что Access Token действителен в течение 15-20 минут
            // Если текущее время минус время создания токена больше 20 минут, считаем токен просроченным
            // В противном случае, токен считается действительным
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(accessToken);

                if (token.ValidTo < DateTime.UtcNow)
                {
                    // Токен истек
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("Check")]
        public string Check()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = GetUserIdFromToken(token);

            return "userId - " + userId + ", Token - " + token;
        }
        private ClaimsIdentity GetIdentity(string id)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, id),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("CheckTokens")]
        public ActionResult CheckTokens(string id, string accessToken, string refreshToken)
        {
            try
            {
                Users user = new Users();
            user = _redisRepository.GetData<Users>(id);// Проверка наличия данных в кэше
            if (user == null) // Данные отсутствуют в кэше, выполняем запрос к базе данных
            {
                user = _context.Users.FirstOrDefault(u => u.access_token == accessToken);
            }
            if (IsTokenExpired(accessToken) && user != null)
            {
                if (IsTokenExpired(refreshToken))
                {
                    return BadRequest(new { Error = "Введите заново емаил и пароль" });
                }
                else
                {
                    _context.Users.Remove(user);
                    _redisRepository.DeleteData("users:" + refreshToken);
                    user.access_token = accessToken = GenerateAccessToken(user.Id);
                    //user.refresh_token = refreshToken = GenerateRefreshToken(user.Id);
                    _context.Users.Update(user);
                    _context.SaveChanges();
                    // Сохранение/обновление данных в кэше на 10 минут
                    _redisRepository.AddOneModel(user);
                    return Ok(accessToken + " \n" + refreshToken); //Токены обновлены
                }
            }
            else if (user != null)
            {
                return Ok(new { Message = "Токен НЕ нуждается в обновлении(Пользователю разрешают войти на страницу)" });
            }
            else
            {
                return BadRequest(new { Error = "Пользователь не найден" });
            }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken?.Claims != null)
                {
                    var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultNameClaimType);

                    if (userIdClaim != null)
                    {
                        return userIdClaim.Value;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
