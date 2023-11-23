using apiServer.Models.Example;
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
        ISolrOperations<Example> solr;

        [HttpPost("NewArticle")]
        public ActionResult NewArticle(int amount) // возвращение статей от новых к старым
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

            var options = new QueryOptions
            {
                OrderBy = new[] { new SortOrder("dataCreate", Order.DESC) },
                Rows = amount // Количество записей, которые вы хотите получить
            };
            List<Example> articles = solr.Query(SolrQuery.All, options);

            return Ok(articles);
        }
        [HttpPost("PopularArticle")]
        public ActionResult PopularArticle(int amount) // возвращение статей от новых к старым
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

            var options = new QueryOptions
            {
                OrderBy = new[] { new SortOrder("views", Order.DESC) },
                Rows = amount // Количество записей, которые вы хотите получить
            };
            List<Example> articles = solr.Query(SolrQuery.All, options);

            return Ok(articles);
        }

    }
}
