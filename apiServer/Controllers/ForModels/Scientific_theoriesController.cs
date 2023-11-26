using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiServer.Controllers.ForModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class Scientific_theoriesController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly RedisScientific_theoriesController _redisScientific_theories;

        public Scientific_theoriesController(ArhivistDbContext context)
        {
            _context = context;
            _redisScientific_theories = new RedisScientific_theoriesController("redis:6379,abortConnect=false");
        }
        [HttpGet("GetSiences")]
        public async Task<ActionResult<IEnumerable<Scientific_theories>>> GetScientific_theories()
        {
            List<Scientific_theories> sciences = _redisScientific_theories.GetAllScientific_theories();
            if (sciences.Count == 0)
            {
                sciences = await _context.Scientific_theories.ToListAsync();
                _redisScientific_theories.AddScientific_theories(sciences);
                return sciences;
            }
            return sciences;
        }
    }
}
