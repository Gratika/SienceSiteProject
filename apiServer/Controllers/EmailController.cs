using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NRediSearch.QueryBuilder;
using StackExchange.Redis;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly RedisEmailController _redisRepository;
        private readonly ArhivistDbContext _context;
        public EmailController( IHttpClientFactory httpClientFactory, ArhivistDbContext context)
        {            
            _httpClient = new HttpClient();
            _redisRepository = new RedisEmailController("redis:6379,abortConnect=false");
            _context = context;
        }
        [HttpGet("SentCode")]
        public async Task<string> SentCode(string email) // обращаемся сюда, если пользователь нажал кнопку "Ещё раз послать код" или просто попал на страницу проверки почты
        {
            string code = GenerateRandomString();
            var emailServiceUrl = $"http://emailservice:80/api/EmailVerification/VerifyEmail?email={email}&code={code}";
            HttpResponseMessage response = await _httpClient.GetAsync(emailServiceUrl);
            string responseContent = await response.Content.ReadAsStringAsync();
            if(responseContent == "1")
            {
                _redisRepository.AddEmail(email, code);
                return responseContent;
            }
            return "0";
        }
        [HttpGet("CheckCode")]
        public async Task<string> CheckCode(string code)
        {
            HashEntry[] userFields = _redisRepository.GetEmail("Email:" + code);
            if (userFields.Length != 0)
            {
                string email = userFields[0].Value;
                // обновление значения в бд check_email
                var recordToUpdate = _context.Users.FirstOrDefault(t => t.email == email); 
                if (recordToUpdate != null)
                {
                    // Изменение значения свойства
                    recordToUpdate.email_is_checked = 1;
                    // Сохранение изменений в базе данных
                    _context.SaveChanges();
                    return "Вы вошли";
                }
               
            }
            return "Вы не вошли";
        }
        [HttpGet("GenerateRandomString")]
        public string GenerateRandomString() // генерация случайной строки
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string randomString = "";

            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                randomString += chars[random.Next(chars.Length)];
            }

            return randomString;
        }
    }
}
