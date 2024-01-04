using apiServer.Controllers.Authentication;
using apiServer.Models;
using apiServer.Models.ForUser;
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

        [HttpPost("AddReaction")]
        public ActionResult AddReaction(Reactions reaction)
        {
            //reaction.article_id = "fec4ce59-2ce9-4398-a2a1-e36af2795af7";
            //reaction.people_id = "eeb84033-8e9a-49c9-bf8e-dc1af18bef57";
            //reaction.reaction_id = "1";
            //Пример

            reaction.Id = Guid.NewGuid().ToString();
            reaction.date_create = DateTime.Now;
            reaction.modified_date = DateTime.Now;
            _context.Reactions.Add(reaction);
            _context.SaveChanges();
            return Ok("Реакция добавленна");
        }
        [HttpGet("GetReactionForArticle")]
        public ArticleAndReactions GetReactionForArticle(string articleId, string emojiId, string peopleId)
        {
            ArticleAndReactions articleAndReactions = new ArticleAndReactions();
            articleAndReactions.CountReactions = _context.Reactions.Where(r => r.reaction_id == emojiId).Count(r => r.article_id == articleId);
            bool IsUserReact = _context.Reactions.Where(r => r.reaction_id == emojiId).Any(r => r.article_id == articleId && r.people_id == peopleId);
            if (IsUserReact == true)
            {
                articleAndReactions.Emotion = _context.Emotions.FirstOrDefault(e => e.Id == emojiId);
            }

            return articleAndReactions;

        }
        [HttpGet("Delete")]
        public void Delete(string articleId)
        {
            var RemoveData = _context.Reactions.Where(r => r.article_id == articleId);
            _context.Reactions.RemoveRange(RemoveData);
            _context.SaveChanges();
        }
    }
}
