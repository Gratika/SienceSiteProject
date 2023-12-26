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
                throw;
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
                throw;
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
                throw;
            }
        }
        [HttpGet("SelectTag")]
        public List<Articles> SelectTag(List<Articles> articles, string? tag) // возвращение статей от новых к старым
        {
            try
            {
                List<Articles> filteredModels = new List<Articles>();
                for (int i = 0; i < articles.Count; i++)
                {
                    string[] Tags = articles[i].tag.Split(',');
                    foreach (string ThisTag in Tags)
                    {
                        if (tag == ThisTag)
                        {
                            filteredModels.Add(articles[i]);
                            break;
                        }
                    }
                }
                
                return filteredModels;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
