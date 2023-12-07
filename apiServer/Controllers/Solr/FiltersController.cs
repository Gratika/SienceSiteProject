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
    public class FiltersController : ControllerBase
    {
        ISolrOperations<Articles> solr;
        private readonly SearchController solrController;
        SearchController searchController;

        public FiltersController(SearchController searchControllerN)
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
            searchController = searchControllerN;
        }
        [HttpGet("OnlySciensceArticles")]
        public ActionResult OnlySciensceArticles(string SearchString) // возвращение статей от новых к старым
        {
            try
            {
                List<Articles> articles = searchController.Search(SearchString);
                var filteredModels = articles.Where(m => !string.IsNullOrEmpty(m.DOI)).ToArray();
                return Ok(filteredModels);
            }   
            catch (Exception ex)
            {
                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
            }      
        }
        [HttpGet("SimpleArticles")]
        public ActionResult SimpleArticles(string SearchString) // возвращение статей от новых к старым
        {
            try
            {
                List<Articles> articles = searchController.Search(SearchString);
                var filteredModels = articles.Where(m => string.IsNullOrEmpty(m.DOI)).ToArray();
                return Ok(filteredModels);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
            }          
        }
    }
}
