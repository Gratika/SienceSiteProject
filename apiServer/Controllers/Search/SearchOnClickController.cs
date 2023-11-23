using apiServer.Models.Example;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet.Commands.Parameters;
using SolrNet;
using CommonServiceLocator;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchOnClickController : ControllerBase
    {
        ISolrOperations<Example> solr;

        [HttpPost("ForScientificArticle")]
        public ActionResult ForScientificArticle(string theory_id ,int amount) // возвращение статей от новых к старым
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

            var options = new QueryOptions
            {
                FilterQueries = new[] { new SolrQueryByField("theory_id", theory_id) },
                Rows = amount // Количество записей, которые вы хотите получить
            };
            List<Example> articles = solr.Query(SolrQuery.All, options);

            return Ok(articles);
        }
    }
}
