using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly ILogger<UserController> _logger;

        public ArticleController(ArhivistDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet(Name = "GetArticles")]
        public async Task<IEnumerable<Articles>> GetArticles()
        {
            return await _context.Articles.ToListAsync();
        }
    }
}
