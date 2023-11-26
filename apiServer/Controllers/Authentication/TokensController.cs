using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace apiServer.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : Controller
    {
        public readonly IConfiguration _configuration; // необходим для доступа к jwt ключу
        public TokensController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GenerateAccessToken")]
        public string GenerateAccessToken()
        {
            try
            {


                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, "username") // Здесь вы можете добавить дополнительные данные пользователя
                }),
                    Expires = DateTime.UtcNow.AddMinutes(1), // Срок действия токена в минутах(15)
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        [HttpGet("GenerateRefreshToken")]
        public string GenerateRefreshToken()
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Expires = DateTime.UtcNow.AddDays(7), // Срок действия токена в днях
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
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
    }
}
