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
        private readonly RedisController _redisSciences;

        public SciencesController(ArhivistDbContext context)
        {
            _context = context;
            _redisSciences = new RedisController("redis:6379,abortConnect=false");
        }
        [HttpGet("GetSiences")]
        public async Task<ActionResult<IEnumerable<Sciences>>> GetSiences()
        {
            try
            {
                    List<Sciences> sciences = await _context.Sciences.ToListAsync();
                    return Ok(sciences);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
