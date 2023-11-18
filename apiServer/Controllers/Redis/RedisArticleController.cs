using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace apiServer.Controllers.Redis
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisArticleController : Controller
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisArticleController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }
        [HttpPost("AddArticle")]
        public void AddArticle(Articles model)
        {
            var hashKey = $"Article:{model.Id}";
            var hashFields = new HashEntry[]
            {
    new HashEntry("Id", model.Id),
    new HashEntry("DOI", model.DOI ?? string.Empty),
    new HashEntry("author_id", model.author_id),
    new HashEntry("title", model.title ?? string.Empty ),
    new HashEntry("tag", model.tag),
    new HashEntry("text", model.text ?? string.Empty),
    new HashEntry("views", model.views),
    new HashEntry("date_created", model.date_created.ToString()),
    new HashEntry("modified_date", model.modified_date.ToString()),
    new HashEntry("theory_id", model.theory_id),
    new HashEntry("path_file", model.path_file ?? string.Empty),
            };

            _database.HashSet(hashKey, hashFields);
        }
        [HttpPost("GetArticle")]
        public Articles GetArticle(string id)
        {
            // Получение хэша из Redis
            HashEntry[] hashFields = _database.HashGetAll($"Article:{id}");

            Articles article = new Articles();
                foreach (var hashField in hashFields)
                {
                    if(hashFields.Length != 0)
                    switch (hashField.Name.ToString())
                    {
                        //выгрузка article
                        case "Id":
                            article.Id = hashField.Value;
                            break;
                        case "DOI":
                            article.DOI = hashField.Value;
                            break;
                        case "author_id":
                            article.author_id = hashField.Value;
                            break;
                        case "title":
                            article.title = hashField.Value;
                            break;
                        case "tag":
                            article.tag = hashField.Value;
                            break;
                        case "text":
                            article.text = hashField.Value;
                            break;
                        case "views":
                            article.views = (int)hashField.Value;
                            break;
                        case "date_created":
                            article.date_created = DateTime.Parse(hashField.Value);
                            break;
                        case "modified_date":
                            article.modified_date = DateTime.Parse(hashField.Value);
                            break;
                        case "theory_id":
                            article.theory_id = (int)hashField.Value;
                            break;
                        case "path_file":
                            article.path_file = hashField.Value;
                            break;
                    }
                }

            return article;
        }
        [HttpPost("GetArticlesForUser")]
        public List<Articles> GetArticlesForUser(string id) // id автора
        {
            List<Articles> articles = new List<Articles>();
            var keys = _redis.GetServer("redis", 6379).Keys();

            // Итерация по всем ключам и получение данных
            foreach (var key in keys)
            {
                string IdForGetArticle = "";
                var userFields = _database.HashGetAll(key);
                // Проверка данных на совпадение              
                foreach (var hashEntry in userFields)
                {
                    if(string.Equals(hashEntry.Name.ToString(), "Id"))
                    {
                        IdForGetArticle = hashEntry.Value;
                    }
                    if (string.Equals(hashEntry.Name.ToString(),"author_id") && string.Equals(hashEntry.Value, id))
                    {
                        articles.Add(GetArticle(IdForGetArticle));
                    }
                }
            }
            return articles;
        }
    }
}
