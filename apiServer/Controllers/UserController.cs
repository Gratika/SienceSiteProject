using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ArhivistDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
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
        public async Task<String> CreateUser()
        {
                Users FirstEx = new Users();
                FirstEx.login = "loginEX";
                FirstEx.password = "passwordEX";
                FirstEx.firstname = "firstnameEX";
                FirstEx.name = "nameEX";
                FirstEx.email = "emailEX";
                FirstEx.birthday = DateTime.Parse("2029-09-20 05:00:00");
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
                // Сохранение пользователя в базе данных
                _context.Users.Add(FirstEx);
                _context.SaveChanges();
                return "Вы успешно зарегистрировались";
            }      
        }
    }
}
