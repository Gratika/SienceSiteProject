using apiServer.Controllers.Authentication;
using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Collections;
using System.Text;
using System.Xml.Linq;

namespace apiServer.Controllers.ForModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmotionController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly ILogger<EmotionController> _logger;
        private readonly RedisController _redisRepository;
        private readonly TokensController _tokens;

        public EmotionController(ArhivistDbContext context, ILogger<EmotionController> logger, TokensController tokens)
        {
            _context = context;
            _logger = logger;
            _tokens = tokens;
            _redisRepository = new RedisController("redis:6379,abortConnect=false");
        }
        // GET: api/Emotions
        [HttpGet("GetAllEmotions")]
        public async Task<ActionResult<IEnumerable<Emotions>>> GetAllEmotions()
        {
            return await _context.Emotions.ToListAsync();
        }
        // GET: api/Emotions/5
        [HttpGet("GetEmotion")]
        public async Task<Emotions> GetEmotion(string key)
        {
            var emotion = await _context.Emotions.FindAsync(key);
            return emotion;
        }
        [HttpGet("RedisGetEmotion")]
        public async Task<ActionResult<Emotions>> RedisFullFuncion()
        {
            try
            {
                // Проверка наличия данных в кэше
                List<Emotions> data = _redisRepository.GetAllData<Emotions>();
                // Данные отсутствуют в кэше, выполняем запрос к источнику данных
                if (data.Count == 0)
                {
                    data = await _context.Emotions.ToListAsync();
                    // Сохранение данных в кэше на 10 минут
                    _redisRepository.AddData(data);
                    return Ok(data);
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
