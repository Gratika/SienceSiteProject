using Microsoft.AspNetCore.Mvc;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateRandomStringController : Controller
    {
        [HttpPost("GenerateRandomString")]
        public string GenerateRandomString(int leght) // генерация случайной строки
        {
            try
            {
                const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
                string randomString = "";

                Random random = new Random();

                for (int i = 0; i < leght; i++)
                {
                    randomString += chars[random.Next(chars.Length)];
                }

                return randomString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
