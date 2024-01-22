using apiServer.Controllers.Authentication;
using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolrNet.Mapping.Validation;

namespace apiServer.Controllers.ForModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionController : ControllerBase
    {
        private readonly ArhivistDbContext _context;

        public ReactionController(ArhivistDbContext context, ILogger<EmotionController> logger, TokensController tokens)
        {
            _context = context;
        }
        //[Authorize]
        [HttpPost("AddReaction")]
        public ActionResult AddReaction(Reactions reaction)
        {
            //reaction.article_id = "da7df965-1419-4516-bae0-5b2dddf12bbb";
            //reaction.people_id = "eeb84033-8e9a-49c9-bf8e-dc1af18bef57";
            //reaction.reaction_id = "1";
            //Пример

            try
            {
                reaction.Id = Guid.NewGuid().ToString();
                reaction.date_create = DateTime.Now;
                reaction.modified_date = DateTime.Now;
                _context.Reactions.Add(reaction);
                _context.SaveChanges();
                return Ok("Реакция добавленна");
            }
            catch (Exception ex)
            {
                throw ex;
            }         
        }
        [HttpGet("GetReactionForArticle")]
        public async Task<FullArticle<T>> GetReactionForArticle<T>(string articleId, string emojiId, string peopleId)
        {
            try
            {
                FullArticle<T> articleAndReactions = new FullArticle<T>();
                articleAndReactions.CountReactions = await _context.Reactions.Where(r => r.reaction_id == emojiId).CountAsync(r => r.article_id == articleId);
                bool IsUserReact = await _context.Reactions.Where(r => r.reaction_id == emojiId).AnyAsync(r => r.article_id == articleId && r.people_id == peopleId);
                if (IsUserReact == true)
                {
                    articleAndReactions.Emotion = await _context.Emotions.FirstOrDefaultAsync(e => e.Id == emojiId);
                }

                return articleAndReactions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //[Authorize]
        [HttpGet("Delete")]
        public void Delete(string articleId)
        {
            try
            {
                var RemoveData = _context.Reactions.Where(r => r.article_id == articleId);
                _context.Reactions.RemoveRange(RemoveData);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
    }
}
