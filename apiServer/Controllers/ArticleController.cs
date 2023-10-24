using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly ILogger<UserController> _logger;

        public ArticleController(ArhivistDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet("GetArticles")]
        public async Task<ActionResult<IEnumerable<Articles>>> Articles()
        {
            //return await _context.Articles.ToListAsync();
            return await _context.Articles
             .Include(a => a.author_) // Включить связанную модель <link>User</link>
                                      //.Select(a => new {
                                      //    Article = a,
                                      //    AuthorEmail = a.author_.email
                                      //})          
             .ToListAsync();
            //.Select(a => a.author_.email)
            //return Ok(result);
        }
        [HttpPost("UploadFile")]
        public async Task<string> UploadFile(IFormFile file)
        {
            Articles article = new Articles();
            article.author_id = 91;
            article.title = "Example2";
            article.tag = "Example2";
            article.text = "Example2";
            article.views = 0;
            article.theory_id = 1;
            article.date_created = DateTime.Now;
            article.modified_date = DateTime.Now;

            // Проверка, что файл был передан
            if (file != null && file.Length > 0)
            {
                // Определение пути для сохранения файла на сервере
                string fileName = Path.GetFileName(file.FileName);
                string filePath = @"C:\Users\Evgen\source\repos\SienceSiteProject\apiServer\Uploads\" + fileName;

                // Сохранение файла на сервере
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    //await file.CToAsync(stream);
                    file.CopyTo(stream);
                }

                // Вызов метода AddFileLink для добавления ссылки на файл в базу данных
                article.path_file = filePath;
                _context.Articles.Add(article);
                _context.SaveChanges();
                // AddFileLink(fileName, filePath);
                return "вы успешно добавили статью";
            }
            return "вы не добавили статью";
        }
    }
}
