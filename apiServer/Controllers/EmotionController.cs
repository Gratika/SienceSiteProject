using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Collections;
using System.Text;
using System.Xml.Linq;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmotionController : ControllerBase
    {
       private readonly ArhivistDbContext _context; // Замість YourDbContext вставте назву вашого контексту бази даних
        private readonly ILogger<EmotionController> _logger;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly RedisRepository _redisRepository;

        public EmotionController(ArhivistDbContext context, ILogger<EmotionController> logger, IConnectionMultiplexer redisConnection)
        {
            _context = context;
            _logger = logger;
            _redisConnection = redisConnection;
            _redisRepository = new RedisRepository("redis:6379,abortConnect=false");
        }
        // GET: api/Emotions
         [HttpGet(Name = "All")]
         public async Task<ActionResult<IEnumerable<Emotions>>> GetEmotions()
         {
             return await _context.Emotions.ToListAsync();
         }             
        // GET: api/Emotions/5
        [HttpGet("{id}")]
        public async Task<Emotions> GetEmotion(int key)
        {
            var emotion = await _context.Emotions.FindAsync(key);
            return emotion;
        }       
        [HttpGet("RedisGetEmotion")]
        public async Task<string> RedisFullFuncion(int emotionId)
        {
            // Проверка наличия данных в кэше
            HashEntry[] userFields = _redisRepository.GetEmotion("emotions:" + emotionId);
            if (userFields.Length != 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var hashEntry in userFields)
                {
                    string fieldName = hashEntry.Name;
                    string fieldValue = hashEntry.Value;

                    sb.AppendLine($"{fieldName}: {fieldValue}");
                }
                return sb.ToString();
            }
            // Данные отсутствуют в кэше, выполняем запрос к источнику данных
            Emotions data = new Emotions();
            data = await GetEmotion(emotionId);
            // Сохранение данных в кэше на 10 минут
            _redisRepository.AddEmotion(data.Id,data.Name,data.Emoji);
            return data.Name + " " + data.Emoji;
        }
    }
}
