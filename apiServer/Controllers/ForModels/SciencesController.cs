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
                List<Sciences> sciences = _redisSciences.GetAllData<Sciences>();
                if (sciences.Count == 0)
                {
                    sciences = await _context.Sciences.ToListAsync();
                    _redisSciences.AddData(sciences);
                    return Ok(sciences);
                }
                return Ok(sciences);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, науки не были найденны - " + ex.Message);
            }
        }
    }
}
