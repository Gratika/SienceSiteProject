using apiServer.Controllers.Authentication;
using apiServer.Controllers.Redis;
using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiServer.Controllers.ForModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class Selected_articlesController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly RedisController _redis;

        public Selected_articlesController(ArhivistDbContext context, TokensController tokens)
        {
            _context = context;
            _redis = new RedisController("redis:6379,abortConnect=false");
        }

        [HttpGet("GetSelectedArticle")]
        public async Task<ActionResult> GetSelectedArticle(string idPeople) //добавление в избранное
        {
            try
            {
                var SelectArticle = _context.Selected_articles.Where(a => a.user_id == idPeople).Include(a => a.user_).Include(a => a.article_).ToList();

                return Ok(SelectArticle);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, статья не была найденна! - " + ex.Message);
            }
        }
        [HttpPost("AddSelectArticle")]
        public async Task<ActionResult> AddSelectArticle(string ArticleId, string UserId) //добавление в избранное
        {
            try
            {
                Selected_articles SelectArticle = new Selected_articles();
                SelectArticle.Id = Guid.NewGuid().ToString();
                SelectArticle.article_id = ArticleId;
                SelectArticle.user_id = UserId;
                SelectArticle.Date_view = DateTime.Now;

                _context.Selected_articles.Add(SelectArticle);
                _context.SaveChanges();
                _redis.AddOneModel(SelectArticle);

                return Ok("Статья успешно добавленна в избранное");
        }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, статья не была добавлена! - " + ex.Message);
    }
}
        [HttpPost("DeleteSelectArticle")]
        public async Task<ActionResult> DeleteSelectArticle(string Id) //добавление в избранное
        {
            try
            {
                Selected_articles SelectArticle = new Selected_articles();
                SelectArticle.Id = Id;
                _context.Selected_articles.Remove(SelectArticle);
                _context.SaveChanges();

                _redis.DeleteData(Id);

                return Ok("Статья удачно удаленна из избранного");
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, статья не была удаленна из избранного - " + ex.Message);
            }
        }
    }
}
