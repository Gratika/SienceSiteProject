using apiServer.Controllers.ForModels;
using apiServer.Controllers.Solr;
using apiServer.Models;
using apiServer.Models.ForUser;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace apiServer.Controllers.Search
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class MainTabArticlesController : ControllerBase
    {
        private readonly ArhivistDbContext _context;
        private readonly ISolrOperations<Articles> solr;
        private readonly SolrArticleController solrArticleController;
        private readonly ReactionController _reactionController;
        private readonly string emojiId;

        public MainTabArticlesController(ArhivistDbContext context, SolrArticleController solrArticleControllerNew, ReactionController reactionController)
        {
            _context = context;
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
            solrArticleController = solrArticleControllerNew;
            _reactionController = reactionController;
            emojiId = "1";
        }
        [HttpGet("NewArticle")]
        public async Task<ActionResult> NewArticle(int amount) // возвращение статей от новых к старым
        {
            try
            {
                var options = new QueryOptions
                {
                    OrderBy = new[] { new SortOrder("date_created", Order.DESC) },
                    Rows = amount, // Количество записей, которые вы хотите получить
                    FilterQueries = new[] { new SolrQuery("IsActive:true") }
                };
                List<Articles> articles = solrArticleController.GetArticle("*:*", options);

                List<FullArticle<Articles>> articleAndReactions = new List<FullArticle<Articles>>();
                foreach (var article in articles)
                {
                    FullArticle<Articles> ar = await _reactionController.GetReactionForArticle<Articles>(article.Id, emojiId, article.author_id);
                    ar.Selected = _context.Selected_articles.Any(a => a.article_id == article.Id && a.people_id == article.author_id);
                    articleAndReactions.Add(new FullArticle<Articles> { Articles = article, Emotion = ar.Emotion, CountReactions = ar.CountReactions, Selected = ar.Selected });
                }

                return Ok(articleAndReactions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("PopularArticle")]
        public async Task<ActionResult> PopularArticle(int amount) // возвращение статей от новых к старым
        {
            try
            {
                var options = new QueryOptions
                {
                    OrderBy = new[] { new SortOrder("views", Order.DESC) },
                    Rows = amount, // Количество записей, которые вы хотите получить
                    FilterQueries = new[] { new SolrQuery("IsActive:true") }
                };
                List<Articles> articles = solrArticleController.GetArticle("*:*", options);

                List<FullArticle<Articles>> articleAndReactions = new List<FullArticle<Articles>>();
                foreach (var article in articles)
                {
                    FullArticle<Articles> ar = await _reactionController.GetReactionForArticle<Articles>(article.Id, emojiId, article.author_id);
                    ar.Selected = _context.Selected_articles.Any(a => a.article_id == article.Id && a.people_id == article.author_id);
                    articleAndReactions.Add(new FullArticle<Articles> { Articles = article, Emotion = ar.Emotion, CountReactions = ar.CountReactions, Selected = ar.Selected });
                }

                return Ok(articleAndReactions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
