using apiServer.Models;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet;
using SolrNet.Commands.Parameters;
using ZstdSharp;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderingController : ControllerBase
    {
        SearchController searchController;

        public OrderingController(SearchController searchControllerN)
        {
            searchController = searchControllerN;
        }
        [HttpGet("FromNewToOld")]
        public List<Articles> FromNewToOld(List<Articles> articles) // возвращение статей от новых к старым
        {
            try
            {
                articles.Sort((x, y) => y.date_created.CompareTo(x.date_created));
                return articles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("FromOldToNew")]
        public List<Articles> FromOldToNew(List<Articles> articles) // возвращение статей от старых к новым
        {
            try
            {
                articles.Sort((x, y) => x.date_created.CompareTo(y.date_created));
                return articles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("ForViews")]
        public List<Articles> ForViews(List<Articles> articles) // возвращение по наибольшему числу просмотров
        {
            try
            {
                articles.Sort((x, y) => y.views.CompareTo(x.views));
                return articles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
