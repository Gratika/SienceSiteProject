using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration; // необходим для доступа к jwt ключу

        public UserController(ArhivistDbContext context, ILogger<UserController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }
        [HttpGet("IsUserUnique")]
        public async /*Task<ActionResult<IEnumerable<Users>>>*/ Task<int> IsUserUnique(Users Person)
        {
            List<Users> users = await _context.Users.ToListAsync();
            foreach (Users user in users)
            {
                if(string.Equals(user.password, Person.password, StringComparison.Ordinal) == true || string.Equals(user.login, Person.login, StringComparison.Ordinal) == true || string.Equals(user.email, Person.email, StringComparison.Ordinal) == true)
                {
                    return 0;
                }
            }
            return 1;          
        }
        [HttpGet("CreateUser")]
        public async Task<String> CreateUser(string email,string password)
        {
                Users FirstEx = new Users();
                FirstEx.password = password;
                FirstEx.email = email;
                FirstEx.date_create = DateTime.Now;
                FirstEx.modified_date = DateTime.Now;
                FirstEx.role_id = 1;
            //Проверка уникальности пользователя по паролю, логину и email
            if (await IsUserUnique(FirstEx) == 0)
            {
                return "Пользователь с таким логином, паролем или email уже существует.";
            }
            else
            {
                //генерируем токены
                var accessToken = GenerateAccessToken();
                var refreshToken = GenerateRefreshToken();
                FirstEx.access_token = accessToken;
                FirstEx.refresh_token = refreshToken;
                // Сохранение пользователя в базе данных
                _context.Users.Add(FirstEx);
                _context.SaveChanges();
                return "Вы успешно зарегистрировались";
            }      
        }
        [HttpGet("CheckTokens")]
        public string CheckTokens(string accessToken,string refreshToken)
        {
            // Поиск пользователя в базе данных по AccessToken пользователя
            Users user = _context.Users.FirstOrDefault(u => u.access_token == accessToken);
                if (IsAccessTokenExpired(accessToken) && user != null)
                {
                    if (IsRefreshTokenExpired(refreshToken))
                    {
                        return "Введите заново емаил и пароль";
                    }
                    else
                    {
                        user.name = "ChangedName";
                        user.access_token = accessToken = GenerateRefreshToken();
                        user.refresh_token = refreshToken = GenerateRefreshToken();
                        _context.SaveChanges();
                        return "Токены обновлены";
                    }
                }
                else
                {
                    return "Токен НЕ нуждается в обновлении";
                }
        }
        [HttpGet("GenerateAccessToken")]
        private string GenerateAccessToken()
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
        [HttpGet("GenerateRefreshToken")]
        private string GenerateRefreshToken()
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
        [HttpGet("IsAccessTokenExpired")]
        // Метод для проверки срока действия Access Token
        private bool IsAccessTokenExpired(string accessToken)
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
        // Метод для проверки срока действия Refresh Token
        private bool IsRefreshTokenExpired(string refreshToken)
        {
            // В данном примере считаем, что Refresh Token действителен в течение нескольких дней или недель
            // Если текущая дата минус время создания токена больше указанного периода, считаем токен просроченным
            // В противном случае, токен считается действительным

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(refreshToken);

            if (token.ValidTo < DateTime.UtcNow)
            {
                // Токен истек
                return true;
            }

            return false;
        }
    }
}
