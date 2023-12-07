using apiServer.Controllers.Solr;
using apiServer.Models;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace apiServer.Controllers.Search
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class MainTabArticlesController : ControllerBase
    {
        ISolrOperations<Articles> solr;
        SolrArticleController solrArticleController;
        public MainTabArticlesController(SolrArticleController solrArticleControllerNew)
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
            solrArticleController = solrArticleControllerNew;
        }
        [HttpGet("NewArticle")]
        public async Task<ActionResult> NewArticle(int amount) // возвращение статей от новых к старым
        {
            try
            {
                var options = new QueryOptions
                {
                    OrderBy = new[] { new SortOrder("date_created", Order.DESC) },
                    Rows = amount // Количество записей, которые вы хотите получить
                };
                List<Articles> articles = solrArticleController.GetArticle("*:*", options);

                return Ok(articles);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
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
                    Rows = amount // Количество записей, которые вы хотите получить
                };
                List<Articles> articles = solrArticleController.GetArticle("*:*", options);

                return Ok(articles);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
            }          
        }
    }
}
