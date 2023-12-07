using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace apiServer.Controllers.Redis
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisArticleController : RedisController
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisArticleController(string connectionString) : base(connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }      
        [HttpPost("GetArticlesForUser")]
        public List<Articles> GetArticlesForUser(string id) // id автора
        {
            List<Articles> articles = GetAllData<Articles>();
            List<Articles> result = new List<Articles>();
            // Итерация по всем ключам и получение данных
            foreach (var article in articles)
            {
                    if (article.author_id == id)
                    {
                        result.Add(article);
                    }
            }
            return articles;
        }
    }
}
