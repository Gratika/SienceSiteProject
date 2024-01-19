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
        [HttpGet("GetScientific_theories")]
        public async Task<ActionResult<IEnumerable<Scientific_theories>>> GetScientific_theories()
        {
            try
            {
                     List<Scientific_theories> sciences = await _context.Scientific_theories.Include(a => a.science_).ToListAsync();
                    return sciences;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
