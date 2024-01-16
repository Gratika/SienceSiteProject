using apiServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace apiServer.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TokensController : Controller
    {
        private ArhivistDbContext _context;
        public readonly IConfiguration _configuration; // необходим для доступа к jwt ключу
        public TokensController(IConfiguration configuration, ArhivistDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("GenerateAccessToken")]
        public string GenerateAccessToken(string email)
        {
            try
            {
                var identity = GetIdentity(email);

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
                throw new Exception();
            }
        }
        [HttpGet("GenerateRefreshToken")]
        public string GenerateRefreshToken(string email)
        {
            try
            {
                var identity = GetIdentity(email);

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
                throw new Exception();
            }
        }

        [HttpGet("IsTokenExpired")]
        // Метод для проверки срока действия Access Token
        public bool IsTokenExpired(string accessToken)
        {
            // В данном примере считаем, что Access Token действителен в течение 15-20 минут
            // Если текущее время минус время создания токена больше 20 минут, считаем токен просроченным
            // В противном случае, токен считается действительным

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken); 

            if (token.ValidTo < DateTime.UtcNow)
            {
                // Токен истек
                return true;
            }
            return false;
        }
        [HttpGet("Check")]        
        public string Check()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            return "userId - " + userId + ", Token - " + token;
        }
        private ClaimsIdentity GetIdentity(string email)
        {
            Users person = _context.Users.FirstOrDefault(x => x.email == email);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.email),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}
