using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Mvc;
using System.Reactive.Linq;
using apiServer.Controllers.Redis;
using System.Net;
using apiServer.Controllers.Solr;
using Microsoft.EntityFrameworkCore;
using apiServer.Controllers.Minio;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using apiServer.Controllers.Authentication;

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
        private readonly ReactionController _reactionController;
        private readonly ImagesController _imagesController;
        private readonly TokensController _tokensController;
        private readonly string emojiId;

        public ArticleController(ArhivistDbContext context, GenerateRandomStringController genericString, FilesController minioController, SolrArticleController searchController, ReactionController reactionController, ImagesController imagesController, TokensController tokensController)
        {
            _context = context;
            _genericString = genericString;
            _httpClient = new HttpClient();
            _minioController = minioController;
            _redisArticleController = new RedisArticleController("redis:6379,abortConnect=false");
            _searchController = searchController;
            _reactionController = reactionController;
            _imagesController = imagesController;
            _tokensController = tokensController;
            emojiId = "1";
        }
        [Authorize]
        [HttpGet("GetArticlesForUser")]
        public async /*Task<ActionResult>*/Task<List<FullArticle<Articles>>> GetArticlesForUser(string id_people/*, string acessToken, string refreshToken*/) // Возвращение статей конкретного пользователя
        {
            try
            {
                //string id = "20a6f9b0-f0dc-44b3-b505-662bd0156104";
                //var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                //if (_tokensController.IsTokenExpired(acessToken))
                //{
                //    return _tokensController.CheckTokens(id, acessToken, refreshToken);

                //}

                List<FullArticle<Articles>> articleAndReactions = new List<FullArticle<Articles>>();
                List<Articles> articles = await _context.Articles.Where(a => a.author_id == id_people).Include(a => a.author_).Include(a => a.theory_).ToListAsync();
                foreach (var article in articles)
                {
                    FullArticle<Articles> ar = await _reactionController.GetReactionForArticle<Articles>(article.Id, emojiId, id_people);
                ar.Selected = _context.Selected_articles.Any(a => a.article_id == article.Id && a.people_id == article.author_id);
                articleAndReactions.Add(new FullArticle<Articles>
                { Articles = article, Emotion = ar.Emotion, CountReactions = ar.CountReactions, Selected = ar.Selected
                });
                }

                return articleAndReactions;
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }
        [Authorize]
        [HttpPost("CreateArticle")]
        public async Task<ActionResult> CreateArticle(Articles? article) // Создание статьи
        {
            //article.author_id = "eeb84033-8e9a-49c9-bf8e-dc1af18bef57";
            //article.title = "12Atrticle";
            //article.tag = "FullNew";
            //article.text = "Example for everyone";
            //article.views = 70;
            //article.theory_id = "2";
            //article.path_file = "";
            try
            {
                article.Id = Guid.NewGuid().ToString();
                article.date_created = DateTime.Now;
                article.modified_date = DateTime.Now;
                article.IsActive = false;
                if (CheckDoiValidity(article.DOI) == false)
                {
                    article.DOI = null;
                }
                _context.Articles.Add(article);
                _context.SaveChanges();

                //добавление данных в Redis
                _redisArticleController.AddOneModel(article);

                _searchController.AddArticle(article);

                ArticlerResponse articlerResponse = new ArticlerResponse();
                //articlerResponse.Articles = await GetArticlesForUser(article.author_id);
                articlerResponse.articleId = article.Id;
                if (article.DOI != null)
                {
                    articlerResponse.Response = "Вы успешно добавили статью ";
                    return Ok(articlerResponse);
                }

                articlerResponse.Response = "Вы успешно добавили статью, но DOI-идентификатор не прошел проверку";
                return Ok(articlerResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetArticle")]
        public async Task<ActionResult<FullArticle<Articles>>> GetArticle(string id, string? peopleId)
        {
            try
            {
                FullArticle<Articles> article = new FullArticle<Articles>();
                article = await _reactionController.GetReactionForArticle<Articles>(id, emojiId, peopleId);
                article.Articles = await _context.Articles.Include(a => a.author_).Include(a => a.theory_).FirstOrDefaultAsync(a => a.Id == id);
                article.Selected = _context.Selected_articles.Any(a => a.article_id == article.Articles.Id && a.people_id == peopleId);

                return Ok(article);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Authorize]
        [HttpPost("RedactArticle")]
        public async Task<ActionResult> RedactArticle(/*IFormFile? file1, IFormFile? file2,*/ Articles article/*, string id, string title, string pathFile, string pathBucket*/)
        {
            try
            {
                article.modified_date = DateTime.Now;

                _redisArticleController.AddOneModel(article);
                _searchController.RedactArticle(article);

                //List<string> FieldsDb = await _minioController.RedactFiles(article.path_file, pathBucket, files);

                //article.path_file = FieldsDb[1];
                _context.Articles.Update(article);
                _context.SaveChanges();

                return Ok("Статья удачно сохраненна");
            }
            catch (Exception ex)
            {
                throw ex;
            }        
        }
        [Authorize]
        [HttpPost("DeleteArticle")]
        public async Task<ActionResult> DeleteArticle(Articles article)
        {
            try
            {
                _searchController.DeleteArticle(article.Id);
                //_redisArticleController.DeleteData(article.Id);
                if (!string.IsNullOrEmpty(article.urls))
                {
                    _imagesController.Delete(article);
                }
                if (!string.IsNullOrEmpty(article.path_file))
                {
                    await _minioController.DeleteFiles(article.Id);
                }
                _context.Articles.Remove(article);
                _context.SaveChanges();

                return Ok("Статья удачно удаленна");
            }
            catch (Exception ex)
            {
                throw ex;
            }       
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
                return false;
            }
        }
        [HttpGet("AddView")]
        public void AddView(string articleId)
        {
            try
            {
                Articles articles = _context.Articles.FirstOrDefault(a => a.Id == articleId);
                articles.views++;
                _context.Articles.Update(articles);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }
        [Authorize]
        [HttpGet("PublicationArticle")]
        public void PublicationArticle(string articleId)
        {
            try
            {
                Articles articles = _context.Articles.FirstOrDefault(a => a.Id == articleId);
                articles.IsActive = true;
                _context.Articles.Update(articles);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }
    }
}
