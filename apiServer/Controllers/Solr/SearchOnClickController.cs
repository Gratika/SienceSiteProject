using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet.Commands.Parameters;
using SolrNet;
using CommonServiceLocator;
using apiServer.Models;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchOnClickController : ControllerBase
    {
        ISolrOperations<Articles> solr;

        public SearchOnClickController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
        }
        [HttpPost("ForScientificArticle")]
        public ActionResult ForScientificArticle(string theory_id ,int amount) // возвращение статей от новых к старым
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();

            var options = new QueryOptions
            {
                FilterQueries = new[] { new SolrQueryByField("theory_id", theory_id) },
                Rows = amount // Количество записей, которые вы хотите получить
            };
            List<Articles> articles = solr.Query(SolrQuery.All, options);

            return Ok(articles);
        }
    }
}
