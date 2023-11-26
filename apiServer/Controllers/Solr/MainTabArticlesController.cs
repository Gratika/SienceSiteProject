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

        public MainTabArticlesController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
        }
        [HttpGet("NewArticle")]
        public async Task<ActionResult> NewArticle(int amount) // возвращение статей от новых к старым
        {
            
            var options = new QueryOptions
            {
                OrderBy = new[] { new SortOrder("date_created", Order.DESC) },
                Rows = amount // Количество записей, которые вы хотите получить
            };
            List<Articles> articles = await solr.QueryAsync(SolrQuery.All, options);

            return Ok(articles);
        }
        [HttpGet("PopularArticle")]
        public async Task<ActionResult> PopularArticle(int amount) // возвращение статей от новых к старым
        {

            var options = new QueryOptions
            {
                OrderBy = new[] { new SortOrder("views", Order.DESC) },
                Rows = amount // Количество записей, которые вы хотите получить
            };
            List<Articles> articles = await solr.QueryAsync(SolrQuery.All, options);

            return Ok(articles);
        }

    }
}
