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
        private readonly RedisController _redis;

        public Scientific_theoriesController(ArhivistDbContext context)
        {
            _context = context;
            _redis = new RedisController("redis:6379,abortConnect=false");
        }
        [HttpGet("GetSiences")]
        public async Task<ActionResult<IEnumerable<Scientific_theories>>> GetScientific_theories()
        {
            try
            {
                List<Scientific_theories> sciences = _redis.GetAllData<Scientific_theories>();
                if (sciences.Count == 0)
                {
                    sciences = await _context.Scientific_theories.Include(a => a.science_).ToListAsync();
                    _redis.AddData(sciences);
                    return sciences;
                }
                return sciences;
            }
           catch (Exception ex)
            {
                return BadRequest("Ошибка, поднауки не были найденны - " + ex.Message);
            }
        }
    }
}
