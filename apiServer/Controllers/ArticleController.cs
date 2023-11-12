using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Tags;
using System.IO.Compression;
using System.Reactive.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using apiServer.Controllers.Redis;
using apiServer.Controllers.Search;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly GenerateRandomStringControlle _genericString;
        private readonly HttpClient _httpClient;
        private readonly MinioController _minioController;
        private readonly RedisArticleController _redisArticleController;
        private readonly SearchController _searchController;

        public ArticleController(ArhivistDbContext context, GenerateRandomStringControlle genericString, MinioController minioController, SearchController searchController)
        {
            _context = context;
            _genericString = genericString;
            _httpClient = new HttpClient();
            _minioController = minioController;
            _redisArticleController = new RedisArticleController("redis:6379,abortConnect=false");
            _searchController = searchController;

        }
        [HttpPost("GetArticlesForUser")]
        public async Task<ActionResult<IEnumerable<Articles>>> GetArticlesForUser(Users user) // Возвращение статей конкретного пользователя
        {
            try
            {
                List<ArticleWithUserTokenModel> articlesWithUserTokens = new List<ArticleWithUserTokenModel>();
                user.email = "zapaspas99@gmail.com"; // пример(удалится)                                                    
                articlesWithUserTokens = _redisArticleController.GetArticlesForUser(user.email);

                if (articlesWithUserTokens == null)
                {


                    articlesWithUserTokens = await _context.Articles
                       .Join(_context.Users,
                       article => article.author_id,
                       user => user.Id,
                       (article, user) => new ArticleWithUserTokenModel
                       {
                           Id = article.Id,
                           DOI = article.DOI,
                           author_id = article.author_id,
                           title = article.title,
                           tag = article.tag,
                           text = article.text,
                           views = article.views,
                           date_created = article.date_created,
                           modified_date = article.modified_date,
                           theory_id = article.theory_id,
                           path_file = article.path_file,
                           login = user.login,
                           name = user.name,
                           email = user.email,
                           firstname = user.firstname,
                           access_token = user.access_token,// Поле access_token из модели User
                           refresh_token = user.refresh_token// Поле refresh_token из модели User
                       })
       .Where(a => a.email == user.email)
       .ToListAsync();
                }

                return Ok(articlesWithUserTokens);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, статья не была выгруженна - " + ex.Message);
            }
        }
        [HttpPost("GetArticle")]
        public async Task<ActionResult<IEnumerable<ArticlerResponse>>> GetArticle(int id) // возвращение конкретной статьи
        {
            try
            {
                ArticleWithUserTokenModel articlesWithUserTokens;
                articlesWithUserTokens = _redisArticleController.GetArticle(id);//выгрузка из редис
                if (articlesWithUserTokens == null)
                {
                    articlesWithUserTokens = GetArticlesFromDb(id); // Выгружаем статью из бд
                }

                return await _minioController.GetArchivWithFiles(articlesWithUserTokens);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, статья не была выгруженна - " + ex.Message);
            }
        }
        [HttpPost("CreateArticle")]
        public async Task<ActionResult> CreateArticle(/*Articles article, IFormFile[]? files*/ IFormFile? file1, IFormFile? file2, IFormFile? file3) // Создание статьи
        {
 Articles article = new Articles(); // создание примера(удалится потом)
            article.Id = 77;
            article.author_id = 94;
 article.title = "Example2";
 article.tag = "Example2";
 article.text = "Example2";
 article.views = 0;
 article.theory_id = 1;
 article.date_created = DateTime.Now;
 article.modified_date = DateTime.Now;
 IFormFile? file4 = file2;
        var files = new List<IFormFile> { file1, file2, file3 };
            try
            {
               
                 // добавить возможность выгрузки из редис
                var user = _context.Users.Find(article.author_id); // Выгружаем данные о авторе статьи из бд

                List<string> FieldsDb = await _minioController.AddFiles(files, article, user.login);

                user.login = FieldsDb[0];
                article.path_file = FieldsDb[1];
                _context.Articles.Add(article);
                _context.SaveChanges();

                //добавление данных в Redis
               // ArticleWithUserTokenModel articlesWithUserTokens = GetArticlesFromDb(96);
                //_redisArticleController.AddArticle(articlesWithUserTokens);

                _searchController.AddArticle(article.title,article.text,article.tag,"имя автора");
             
                return Ok(new { Message = "Вы успешно добавили статью " });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Вы не добавили статью - {ex}" });
            }
        }
        [HttpGet("GetArticlesFromDb")]
        public ArticleWithUserTokenModel GetArticlesFromDb(int id)
        {

            var articlesWithUserTokens = _context.Articles
                .Join(_context.Users,
                article => article.author_id,
                user => user.Id,
                (article, user) => new ArticleWithUserTokenModel
                {
                Id = article.Id,
        DOI = article.DOI,
        author_id = article.author_id,
        title = article.title,
        tag = article.tag,
        text = article.text,
        views = article.views,
        date_created = article.date_created,
        modified_date = article.modified_date,
        theory_id = article.theory_id,
        path_file = article.path_file,
        login = user.login,
        name = user.name,
        firstname = user.firstname,
        access_token = user.access_token,
        refresh_token = user.refresh_token
    })
    .FirstOrDefault(a => a.Id == id);

            return articlesWithUserTokens;
        }
    }
}
