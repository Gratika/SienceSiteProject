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

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly RedisEmailController _redisRepository;
        private readonly ArhivistDbContext _context;
        private readonly GenerateRandomStringControlle _genericString;
        public EmailController( IHttpClientFactory httpClientFactory, ArhivistDbContext context, GenerateRandomStringControlle genericString)
        {            
            _httpClient = new HttpClient();
            _redisRepository = new RedisEmailController("redis:6379,abortConnect=false");
            _context = context;
            _genericString = genericString;
        }
        [HttpGet("SentCode")]
        public async Task<string> SentCode(string email) // обращаемся сюда, если пользователь нажал кнопку "Ещё раз послать код" или просто попал на страницу проверки почты
        {
            try
            {
                string code = _genericString.GenerateRandomString(4);
                var emailServiceUrl = $"http://emailservice:80/api/EmailVerification/VerifyEmail?email={email}&code={code}";
                HttpResponseMessage response = await _httpClient.GetAsync(emailServiceUrl);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent == "1")
                {
                    _redisRepository.AddEmail(email, code);
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
        [HttpPost("CheckCode")]
        public async Task<ActionResult> CheckCode(string code)
        {
            try
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
                        return Ok("Вы вошли");
                    }

                }
                return BadRequest(new { Error = "Вы не вошли" });
            }
            catch (Exception ex)
            {
                BadRequest(new { Error = "Вы не вошли - " + ex.Message});
            }
            return BadRequest();
        }
        //[HttpGet("GenerateRandomString")]
        //public string GenerateRandomString() // генерация случайной строки
        //{
        //    try
        //    {
        //        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //        string randomString = "";

        //        Random random = new Random();

        //        for (int i = 0; i < 4; i++)
        //        {
        //            randomString += chars[random.Next(chars.Length)];
        //        }

        //        return randomString;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw new Exception();
        //    }
        //}
    }
}
