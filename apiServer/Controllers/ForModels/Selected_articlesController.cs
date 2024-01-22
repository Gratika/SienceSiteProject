using apiServer.Controllers.Authentication;
using apiServer.Controllers.Redis;
using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace apiServer.Controllers.ForModels
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class Selected_articlesController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly RedisController _redis;
        private readonly ReactionController _reactionController;
        private readonly string emojiId;

        public Selected_articlesController(ArhivistDbContext context, ReactionController reactionController)
        {
            _context = context;
            _redis = new RedisController("redis:6379,abortConnect=false");
            _reactionController = reactionController;
            emojiId = "1";
        }
        [HttpGet("GetSelectedArticle")]
        public async Task<ActionResult> GetSelectedArticle(string idPeople, int page) //добавление в избранное
        {
            try
            {
                var selArticle = await _context.Selected_articles.Where(a => a.people_id == idPeople).Include(a => a.people_).Include(a => a.article_).Skip(page * 10).Take(10).ToListAsync();

                SearchResponse<FullArticle<Selected_articles>> articlesAndReactions = new SearchResponse<FullArticle<Selected_articles>>();
                articlesAndReactions.Articles = new List<FullArticle<Selected_articles>>();
                articlesAndReactions.allPages =  (double)Math.Ceiling((decimal)await _context.Selected_articles.Where(a => a.people_id == idPeople).CountAsync() / 10);  

                foreach (var oneSelectArticles in selArticle)
                {

                    FullArticle<Articles> ar = await _reactionController.GetReactionForArticle<Articles>(oneSelectArticles.Id, emojiId, oneSelectArticles.article_.author_id);
                    ar.Selected = _context.Selected_articles.Any(a => a.article_id == oneSelectArticles.Id && a.people_id == oneSelectArticles.article_.author_id);
                    articlesAndReactions.Articles.Add(new FullArticle<Selected_articles> { Articles = oneSelectArticles, Emotion = ar.Emotion, CountReactions = ar.CountReactions, Selected = ar.Selected });

                }
                return Ok(articlesAndReactions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("AddSelectArticle")]
        public async Task<ActionResult> AddSelectArticle([FromForm] string ArticleId, [FromForm] string PeopleId) //добавление в избранное
        {
            try
            {
                Selected_articles SelectArticle = new Selected_articles();
                SelectArticle.article_id = ArticleId;
                SelectArticle.people_id = PeopleId;
                SelectArticle.Id = Guid.NewGuid().ToString();
                SelectArticle.Date_view = DateTime.Now;

                _context.Selected_articles.Add(SelectArticle);
                _context.SaveChanges();
                _redis.AddOneModel(SelectArticle);

                return Ok("Статья успешно добавленна в избранное");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("DeleteSelectArticle")]
        public async Task<ActionResult> DeleteSelectArticle(string articleId, string peopleId) //добавление в избранное
        {
            try
            {
                var SelectArticle = _context.Selected_articles.FirstOrDefault(a => a.article_id == articleId && a.people_id == peopleId);
                //Selected_articles SelectArticle = new Selected_articles();
                //SelectArticle.article_id = articleId;
                //SelectArticle.people_id = peopleId;
                _context.Selected_articles.Remove(SelectArticle);
                _context.SaveChanges();

                _redis.DeleteData(SelectArticle.Id);

                return Ok("Статья удачно удаленна из избранного");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
