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
    public class OrderingController : ControllerBase
    {
        ISolrOperations<Articles> solr;
        SearchController searchController;

        public OrderingController(SearchController searchControllerN)
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
            searchController = searchControllerN;
        }
        [HttpGet("FromNewToOld")]
        public ActionResult FromNewToOld(string SearchString) // возвращение статей от новых к старым
        {
            try
            {
                List<Articles> articles = searchController.Search(SearchString);
                articles.Sort((x, y) => y.date_created.CompareTo(x.date_created));
                return Ok(articles);
            }
            catch (Exception ex)
            {

                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
            }
            
        }
        [HttpGet("FromOldToNew")]
        public ActionResult FromOldToNew(string SearchString) // возвращение статей от старых к новым
        {
            try
            {
                List<Articles> articles = searchController.Search(SearchString);
                articles.Sort((x, y) => x.date_created.CompareTo(y.date_created));
                return Ok(articles);
            }
           catch(Exception ex)
            {
                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
            }
        }
        [HttpGet("ForViews")]
        public ActionResult ForViews(string SearchString) // возвращение по наибольшему числу просмотров
        {
            try
            {
                List<Articles> articles = searchController.Search(SearchString);
                articles.Sort((x, y) => y.views.CompareTo(x.views));
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
            }
        }
    }
}
