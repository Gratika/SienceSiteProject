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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        private readonly RedisPeopleController _redisPeopleController;

        public ArticleController(ArhivistDbContext context, GenerateRandomStringControlle genericString, MinioController minioController, SearchController searchController)
        {
            _context = context;
            _genericString = genericString;
            _httpClient = new HttpClient();
            _minioController = minioController;
            _redisArticleController = new RedisArticleController("redis:6379,abortConnect=false");
            _searchController = searchController;
            _redisPeopleController = new RedisPeopleController("redis:6379,abortConnect=false");
        }
        [HttpPost("GetArticlesForUser")]
        public async Task<ActionResult<ArticleAndPeople>> GetArticlesForUser(/*Users user*/string id_people) // Возвращение статей конкретного пользователя
        {
            try
            {
                ArticleAndPeople articleAndPeople = new ArticleAndPeople();
                articleAndPeople.article = _redisArticleController.GetArticlesForUser(id_people);            
                if (articleAndPeople.article.Count != 0)
                {
                    articleAndPeople.people = _redisPeopleController.GetPeople(articleAndPeople.article[0].author_id);
                    if (string.IsNullOrEmpty(articleAndPeople.people.Id) == false)
                    {                    
                        return Ok("Из редиса - " + articleAndPeople.article[0].title + articleAndPeople.people.Id);
                    }       
                    
                }
                if(articleAndPeople.article.Count == 0 || string.IsNullOrEmpty(articleAndPeople.people.Id) == true)
                {
                                                                        // Получаем статьи пользователя и соответствующие данные пользователя по его почте
                    var articlesQuery = from article in _context.Articles
                                        join people in _context.people on article.author_id equals people.Id
                                        where people.Id == id_people
                                        select new
                                        {
                                            Article = article,
                                            People = people
                                        };


                    // Преобразуем результат в список объектов Article, объект User и объект People             
                    articleAndPeople.article = new List<Articles>
                {
                articlesQuery.Select(a => a.Article).FirstOrDefault() //добавляем одну найденную статью
                };
                    articleAndPeople.people = articlesQuery.Select(a => a.People).FirstOrDefault();

                    // Возвращаем результат в представление или обрабатываем дальше
                    return Ok(articleAndPeople);
                }
                return BadRequest("Ошибка, статьи не были обнаруженны");
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, статьи не были обнаруженны - " + ex.Message);
            }
        }
        
        [HttpPost("CreateArticle")]
        public async Task<ActionResult> CreateArticle(Articles article, List<IFormFile>? files /*IFormFile? file1, IFormFile? file2, IFormFile? file3, string id_people*/) // Создание статьи
        {
            //Articles article = new Articles(); // создание примера(удалится потом)
            //article.Id = Guid.NewGuid().ToString();
            //article.author_id = id_people;
            //article.title = "Example2";
            //article.tag = "Example2";
            //article.text = "Example2";
            //article.views = 0;
            //article.theory_id = 1;
            //article.date_created = DateTime.Now;
            //article.modified_date = DateTime.Now;
            //IFormFile? file4 = file2;
            //var files = new List<IFormFile> { file1, file2, file3 };
            try
            {
                // добавить возможность выгрузки из редис
                People people = _context.people.FirstOrDefault(p => article.author_id == p.Id); // Выгружаем данные о авторе статьи из бд для взятия bucket_path               
                //people.path_bucket = _genericString.GenerateRandomString(7);
                List<string> FieldsDb = await _minioController.AddFiles(files, article, people.path_bucket);

                people.path_bucket = FieldsDb[0];
                article.path_file = FieldsDb[1];
                _context.Articles.Add(article);
                _context.SaveChanges();

                //добавление данных в Redis
                _redisArticleController.AddArticle(article);

                _searchController.AddArticle( article /*article.title,article.text,article.tag,"имя автора",article.views,article.DOI*/);
             
                return Ok(new { Message = "Вы успешно добавили статью " });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Вы не добавили статью - {ex}" });
            }
        }
        [HttpGet("GetArticle")]
        public async Task<ActionResult<ArticleAndPeople>> GetArticle(string id)
        {
            ArticleAndPeople articleAndPeople = new ArticleAndPeople();
            articleAndPeople.article = new List<Articles>
            {
                _redisArticleController.GetArticle(id)
            };
            if (articleAndPeople.article.Count != 0)
            {
                articleAndPeople.people = _redisPeopleController.GetPeople(articleAndPeople.article[0].author_id);               
                if (string.IsNullOrEmpty(articleAndPeople.people.Id) == false)
                {
                    return articleAndPeople;
                }

            }
            if (articleAndPeople.article.Count == 0 || string.IsNullOrEmpty(articleAndPeople.people.Id) == true)
            {


                // Получаем статьи пользователя и соответствующие данные пользователя по его id
                var articlesQuery = from article in _context.Articles
                                    join people in _context.people on article.author_id equals people.Id
                                    where id == article.Id
                                    select new
                                    {
                                        Article = article,
                                        People = people
                                    };
                // Преобразуем результат в список объектов Article, объект User и объект People
                //ArticleAndPeople articleAndPeople = new ArticleAndPeople();
                articleAndPeople.article = new List<Articles>
                {
                articlesQuery.Select(a => a.Article).FirstOrDefault() //добавляем одну найденную статью
                };
                articleAndPeople.people = articlesQuery.Select(a => a.People).FirstOrDefault();
            }
            
            return articleAndPeople;
        }
        [HttpPost("RedactArticle")]
        public async Task<ActionResult> RedactArticle(Articles? article)
        {
            article.Id = "e35f9795-e8c0-4ac2-b294-f81c16e6157d";
            article.author_id = "eeb84033-8e9a-49c9-bf8e-dc1af18bef57";
            article.title = "ReadactExample2";
            article.tag = "Example2";
            article.text = "Example2";
            article.views = 0;
            article.theory_id = 1;
            article.date_created = new DateTime(2023, 11, 17, 17, 16, 16); //ПРИМЕР

            article.modified_date = DateTime.Now;
            _context.Articles.Update(article);
            _context.SaveChanges();

            return Ok("Статья удачно сохраненна");
        }
        [HttpPost("DeleteArticle")]
        public async Task<ActionResult> DeleteArticle(Articles? article)
        {
            article.Id = "e35f9795-e8c0-4ac2-b294-f81c16e6157d";
            article.author_id = "eeb84033-8e9a-49c9-bf8e-dc1af18bef57";
            article.title = "ReadactExample2";
            article.tag = "Example2";
            article.text = "Example2";
            article.views = 0;
            article.theory_id = 1;
           // article.date_created = new DateTime(2023, 11, 17, 17, 16, 16); //ПРИМЕР
            //article.modified_date = DateTime.Now;

            _context.Articles.Remove(article);
            _context.SaveChanges();
            _searchController.DeleteDocumentsWithInvalidTitle(article.Id);


            return Ok("Статья удачно удаленна");
        }

    }
}
