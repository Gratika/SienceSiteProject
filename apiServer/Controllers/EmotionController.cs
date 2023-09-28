using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace apiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmotionController : ControllerBase
    {
       private readonly ArhivistDbContext _context; // Замість YourDbContext вставте назву вашого контексту бази даних
        private readonly ILogger<EmotionController> _logger;

        public EmotionController(ArhivistDbContext context, ILogger<EmotionController> logger) // Тут також вставте назву вашого контексту бази даних
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Emotions
         [HttpGet(Name ="All")]

         public async Task<ActionResult<IEnumerable<Emotions>>> GetEmotions()
         {
             return await _context.Emotions.ToListAsync();
         }

        // GET: api/Emotions
        /*[HttpGet(Name = "All")]

        public  ActionResult<IEnumerable<string>> GetEmotions()
        {
            return new string[] { "Value 1", "Value 2" };
        }*/

       /* [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Value 1", "Value 2" };
        }*/

        // GET: api/Emotions/5
       /* [HttpGet("{id}")]
        public async Task<ActionResult<Emotions>> GetEmotion(int id)
        {
            var emotion = await _context.Emotions.FindAsync(id);

            if (emotion == null)
            {
                return NotFound();
            }

            return emotion;
        }*/
    }
}
