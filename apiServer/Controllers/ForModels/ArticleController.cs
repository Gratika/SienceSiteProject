using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Mvc;
using System.Reactive.Linq;
using apiServer.Controllers.Redis;
using System.Net;
using apiServer.Controllers.Solr;
using Microsoft.EntityFrameworkCore;

namespace apiServer.Controllers.ForModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly GenerateRandomStringController _genericString;
        private readonly HttpClient _httpClient;
        private readonly FilesController _minioController;
        private readonly RedisArticleController _redisArticleController;
        private readonly SolrArticleController _searchController;

        public ArticleController(ArhivistDbContext context, GenerateRandomStringController genericString, FilesController minioController, SolrArticleController searchController)
        {
            _context = context;
            _genericString = genericString;
            _httpClient = new HttpClient();
            _minioController = minioController;
            _redisArticleController = new RedisArticleController("redis:6379,abortConnect=false");
            _searchController = searchController;
        }
        [HttpGet("GetArticlesForUser")]
        public async Task<ActionResult<ArticleAndPeople>> GetArticlesForUser(string id_people) // Возвращение статей конкретного пользователя
        {
            try
            {
                List<Articles> article = new List<Articles>();
                //article = _redisArticleController.GetArticlesForUser(id_people);
                //if (article.Count == 0)
                //{
                article = await _context.Articles.Where(a => a.author_id == id_people).Include(a => a.author_).Include(a => a.theory_).ToListAsync();
                //}
                return Ok(article);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, статьи не были обнаруженны - " + ex.Message);
            }
        }

        [HttpPost("CreateArticle")]
        public async Task<ActionResult> CreateArticle(Articles? article) // Создание статьи
        {
            article.Id = Guid.NewGuid().ToString();
            article.author_id = "eeb84033-8e9a-49c9-bf8e-dc1af18bef57";
            article.title = "Example2";
            article.tag = "Example2";
            article.text = "Example2";
            article.views = 150;
            article.theory_id = "2";
            article.date_created = DateTime.Now;
            article.modified_date = DateTime.Now;
            //try
            //{
                if (CheckDoiValidity(article.DOI) == false)
                {
                    article.DOI = null;
                }
                _context.Articles.Add(article);
                _context.SaveChanges();

                //добавление данных в Redis
                _redisArticleController.AddOneModel(article);

                _searchController.AddArticle(article);

                if (article.DOI != null)
                {
                    return Ok(new { Message = "Вы успешно добавили статью " });
                }

                return Ok(new { Message = "Вы успешно добавили статью, но DOI-идентификатор не прошел проверку" });
        //}
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Error = $"Вы не добавили статью - {ex.Message}" });
        //    }
        }
        [HttpGet("GetArticle")]
        public async Task<ActionResult<Articles>> GetArticle(string id)
        {
            try
            {
                Articles article = _redisArticleController.GetData<Articles>(id);
                if(string.IsNullOrEmpty(article.Id) == true)
                {
                    article = await _context.Articles.Include(a => a.author_).Include(a => a.theory_).FirstOrDefaultAsync(a => a.Id == id);
                }

                return Ok(article);
            }
            catch (Exception ex)
            {
                return Ok("Ошибка, не удалось найти статью - " + ex.Message);
            }
        }
        [HttpPost("RedactArticle")]
        public async Task<ActionResult> RedactArticle(/*IFormFile? file1, IFormFile? file2,*/ Articles article/*, string id, string title, string pathFile, string pathBucket*/)
        {
            //Articles article = new Articles();
            //article.Id = id;
            //article.author_id = "eeb84033-8e9a-49c9-bf8e-dc1af18bef57";
            //article.title = title;
            //article.tag = "Example2";
            //article.text = "Example2";
            //article.views = 0;
            //article.theory_id = "1";
            //article.path_file = pathFile;
            //article.date_created = new DateTime(2023, 11, 17, 17, 16, 16); //ПРИМЕР
            //var files = new List<IFormFile> { file1, file2 };
            article.modified_date = DateTime.Now;

            _redisArticleController.AddOneModel(article);
            _searchController.RedactArticle(article);

            //List<string> FieldsDb = await _minioController.RedactFiles(article.path_file, pathBucket, files);

            //article.path_file = FieldsDb[1];
            _context.Articles.Update(article);
            _context.SaveChanges();

            return Ok("Статья удачно сохраненна");
        }
        [HttpPost("DeleteArticle")]
        public async Task<ActionResult> DeleteArticle(Articles? article,/*string? pathFile,*/string? pathBucket/*, string id*/)
        {
            //article.Id = id;
            //article.author_id = "eeb84033-8e9a-49c9-bf8e-dc1af18bef57";
            //article.title = "ReadactExample2";
            //article.tag = "Example2";
            //article.text = "Example2";
            //article.views = 0;
            //article.theory_id = "1";
            //article.path_file = pathFile;
            // article.date_created = new DateTime(2023, 11, 17, 17, 16, 16); //ПРИМЕР
            //article.modified_date = DateTime.Now;

            _context.Articles.Remove(article);
            _context.SaveChanges();

            _searchController.DeleteArticle(article.Id);
            _redisArticleController.DeleteData(article.Id);
            if (string.Equals(pathBucket, "") == false || string.Equals(/*pathFile*/article.path_file, "") == false)
            {
                _minioController.DeleteFiles(article.path_file, pathBucket);
            }


            return Ok("Статья удачно удаленна");
        }
        [HttpGet("CheckDoiValidity")]
        public bool CheckDoiValidity(string doi)
        {
            try
            {
                // Формирование URL-адреса запроса к базе данных <link>CrossRef</link>
                string url = $"https://api.crossref.org/works/{doi}";

                // Создание объекта WebClient для выполнения запроса
                using (WebClient client = new WebClient())
                {
                    // Отправка GET-запроса и получение ответа в виде JSON
                    string response = client.DownloadString(url);

                    // Проверка наличия поля "status" со значением "ok" в ответе
                    if (response.Contains("\"status\":\"ok\""))
                    {
                        return true; // DOI идентификатор действителен
                    }
                    else
                    {
                        return false; // DOI идентификатор недействителен
                    }
                }
            }
            catch (Exception ex)
            {
                return false; // Произошла ошибка при проверке DOI
            }
        }

    }
}
