﻿using apiServer.Controllers.Redis;
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
        private readonly ArhivistDbContext _context;
        private readonly ILogger<EmotionController> _logger;
        private readonly RedisEmotionController _redisRepository;
        private readonly TokensController _tokens;

        public EmotionController(ArhivistDbContext context, ILogger<EmotionController> logger, TokensController tokens)
        {
            _context = context;
            _logger = logger;
            _tokens = tokens;
            _redisRepository = new RedisEmotionController("redis:6379,abortConnect=false");
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
        [HttpPost("RedisGetEmotion")]
        public async Task<ActionResult<Emotions>> RedisFullFuncion()
        {
            try
            {
                // Проверка наличия данных в кэше
                List<Emotions> data = _redisRepository.GetAllEmotions();
                // Данные отсутствуют в кэше, выполняем запрос к источнику данных
                if(data.Count == 0)
                {
                    data = await _context.Emotions.ToListAsync();
                    // Сохранение данных в кэше на 10 минут
                    _redisRepository.AddEmotion(data);
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
