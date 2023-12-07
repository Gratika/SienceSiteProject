using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet.Commands.Parameters;
using SolrNet;
using CommonServiceLocator;
using apiServer.Models;
using apiServer.Controllers.Solr;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchOnClickController : ControllerBase
    {
        SolrArticleController solrArticleController;

        public SearchOnClickController( SolrArticleController solrArticleControllerNew)
        {
            solrArticleController = solrArticleControllerNew;
        }
        [HttpGet("ForScientificArticle")]
        public ActionResult ForScientificArticle(string theory_id ,int amount) // возвращение статей от новых к старым
        {
            try
            {
                var options = new QueryOptions
                {
                    FilterQueries = new[] { new SolrQueryByField("theory_id", theory_id) },
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
