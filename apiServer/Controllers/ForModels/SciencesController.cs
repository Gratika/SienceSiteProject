using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiServer.Controllers.ForModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class SciencesController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly RedisSciencesController _redisSciences;

        public SciencesController(ArhivistDbContext context)
        {
            _context = context;
            _redisSciences = new RedisSciencesController("redis:6379,abortConnect=false");
        }
        [HttpGet("GetSiences")]
        public async Task<ActionResult<IEnumerable<Sciences>>> GetSiences()
        {
            List<Sciences> sciences = _redisSciences.GetAllSciences();
            if (sciences.Count == 0)
            {
                sciences = await _context.Sciences.ToListAsync();
                _redisSciences.AddSciences(sciences);
                return sciences;
            }
            return sciences;
        }
    }
}
