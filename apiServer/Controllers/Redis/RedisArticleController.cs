using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace apiServer.Controllers.Redis
{
    public class RedisArticleController : Controller
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisArticleController(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }
        public void AddArticle(ArticleWithUserTokenModel model)
        {

            var hashKey = $"Article:{model.Id}";
            var hashFields = new HashEntry[]
            {
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
    new HashEntry("login", model.login ?? string.Empty),
    new HashEntry("name", model.name ?? string.Empty),
    new HashEntry("firstname", model.firstname ?? string.Empty),
    new HashEntry("access_token", model.access_token),
    new HashEntry("refresh_token", model.refresh_token),
    new HashEntry("email", model.email),
            };

            _database.HashSet(hashKey, hashFields);
        }
        public ArticleWithUserTokenModel GetArticle(int id)
        {
            // Получение хэша из Redis
            HashEntry[] hashFields = _database.HashGetAll($"Article:{id}");

            ArticleWithUserTokenModel article = new ArticleWithUserTokenModel();
            foreach (var hashField in hashFields)
            {
                switch (hashField.Name)
                {
                    case "Id":
                        article.Id = Convert.ToInt32(hashField.Value);
                        break;
                    case "DOI":
                        article.DOI = hashField.Value;
                        break;
                    case "author_id":
                        article.author_id = (int)hashField.Value;
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
                    case "login":
                        article.login = hashField.Value;
                        break;
                    case "name":
                        article.name = hashField.Value;
                        break;
                    case "firstname":
                        article.firstname = hashField.Value;
                        break;
                    case "access_token":
                        article.access_token = hashField.Value;
                        break;
                    case "refresh_token":
                        article.refresh_token = hashField.Value;
                        break;
                    case "email":
                        article.email = hashField.Value;
                        break;
                }
            }

            return article;
        }
        public List<ArticleWithUserTokenModel> GetArticlesForUser(string email)
        {
            List<ArticleWithUserTokenModel> articles = new List<ArticleWithUserTokenModel>();
            var keys = _redis.GetServer("redis", 6379).Keys();

            // Итерация по всем ключам и получение данных
            foreach (var key in keys)
            {
                var userFields = _database.HashGetAll(key);
                // Проверка данных на совпадение              
                foreach (var hashEntry in userFields)
                {
                    if (hashEntry.Name.ToString() == "email" && hashEntry.Value == email)
                    {
                        articles.Add(new ArticleWithUserTokenModel());
                    }
                }
            }
            return null;
        }
    }
}
