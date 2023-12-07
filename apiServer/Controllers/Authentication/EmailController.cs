using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
//using NRediSearch.QueryBuilder;
using StackExchange.Redis;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;

namespace apiServer.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly RedisController _redisRepository;
        private readonly ArhivistDbContext _context;
        private readonly GenerateRandomStringController _genericString;
        public EmailController(IHttpClientFactory httpClientFactory, ArhivistDbContext context, GenerateRandomStringController genericString)
        {
            _httpClient = new HttpClient();
            _redisRepository = new RedisController("redis:6379,abortConnect=false");
            _context = context;
            _genericString = genericString;
        }
        [HttpGet("SentCode")]
        public async Task<string> SentCode(string email) // обращаемся сюда, если пользователь нажал кнопку "Ещё раз послать код" или просто попал на страницу проверки почты
        {
            try
            {
                Email EmailAndCode = new Email();
                EmailAndCode.Emaill = email;
                EmailAndCode.Id = _genericString.GenerateRandomString(4);
                var emailServiceUrl = $"http://emailservice:80/api/EmailVerification/VerifyEmail?email={email}&code={EmailAndCode.Id}";
                HttpResponseMessage response = await _httpClient.GetAsync(emailServiceUrl);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent == "1")
                {
                    _redisRepository.AddOneModel(EmailAndCode);
                    return responseContent;
                }
                return "0";
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return "0";
        }
        [HttpGet("CheckCode")]
        public async Task<ActionResult> CheckCode(string code)
        {
            try
            {
                Email EmailAndCode = _redisRepository.GetData<Email>(code);
                if (EmailAndCode.Id.Length != 0)
                {
                    // обновление значения в бд check_email
                    var recordToUpdate = _context.Users.FirstOrDefault(t => t.email == EmailAndCode.Emaill);
                    if (recordToUpdate != null)
                    {
                        // Изменение значения свойства
                        recordToUpdate.email_is_checked = 1;
                        // Сохранение изменений в базе данных
                        _context.SaveChanges();
                        return Ok("Вы вошли");
                    }

                }
                return BadRequest(new { Error = "Вы не вошли" });
            }
            catch (Exception ex)
            {
                BadRequest(new { Error = "Вы не вошли - " + ex.Message });
            }
            return BadRequest("Вы не вошли");
        }
    }
}
