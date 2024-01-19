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
        private readonly SearchController solrController;
        SearchController searchController;

        public FiltersController(SearchController searchControllerN)
        {
            searchController = searchControllerN;
        }
        [HttpGet("OnlySciensceArticles")]
        public List<Articles> OnlySciensceArticles(List<Articles> articles) // возвращение статей от новых к старым
        {
            try
            {
                var filteredModels = articles.Where(m => !string.IsNullOrEmpty(m.DOI)).ToList();
                return filteredModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("SimpleArticles")]
        public List<Articles> SimpleArticles(List<Articles> articles) // возвращение статей от новых к старым
        {
            try
            {              
                var filteredModels = articles.Where(m => string.IsNullOrEmpty(m.DOI)).ToList();
                return filteredModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("SelectYear")]
        public List<Articles> SelectYear(List<Articles> articles, int? year) // возвращение статей от новых к старым
        {
            try
            {
                var filteredModels = articles.Where(m => m.date_created.Year == year).ToList();
                return filteredModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }     
    }
}
