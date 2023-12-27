using apiServer.Controllers.Search;
using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace apiServer.Controllers.Solr
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuSearchingController : ControllerBase
    {
        SearchController _searchController;
        OrderingController _orderingController;
        FiltersController _filtersController;
        public MenuSearchingController(SearchController searchController, OrderingController orderingController, FiltersController filtersController)
        {
            _searchController = searchController;
            _orderingController = orderingController;
            _filtersController = filtersController;
        }

        [HttpPost("SearchWithFilters")]
        public ActionResult<List<Articles>> SearchWithFilters(string SearchString, int Pages,int? year, List<int>? Filters, int? TypeOrder,string? tags) // возвращение по наибольшему числу просмотров
        {
            try
            {
                //Filters = new List<int>();
                //Filters.Add(1);
                SearchResponse searchResponse = SearchWithOrders(SearchString, Pages * 10, TypeOrder, tags);
                for (int i = 0; i < Filters.Count; i++)
                {
                    switch (Filters[i])
                    {
                        case 1:
                            searchResponse.Articles = _filtersController.OnlySciensceArticles(searchResponse.Articles);
                            break;
                        case 2:
                            searchResponse.Articles = _filtersController.SimpleArticles(searchResponse.Articles);
                            break;
                    }
                }
                if (year != null)
                {
                    searchResponse.Articles = _filtersController.SelectYear(searchResponse.Articles, year);
                }
                //if (string.IsNullOrEmpty(tags) == false)
                //{
                //    searchResponse.Articles = _filtersController.SelectTag(searchResponse.Articles,tags);
                //}

            return Ok(searchResponse);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
    }
}
        [HttpGet("SearchWithOrdersAndTags")]
        public SearchResponse SearchWithOrders(string SearchString, int Pages, int? TypeOrder, string? tags) // возвращение по наибольшему числу просмотров
        {
            List<Articles> articles = _searchController.Search(SearchString, tags);
                switch (TypeOrder)
                {
                    case 10:
                        articles = _orderingController.FromNewToOld(articles);
                        break;
                    case 11:
                        articles = _orderingController.FromOldToNew(articles);
                        break;
                    case 12:
                        articles = _orderingController.ForViews(articles);
                        break;
                }
            List<Articles> tenArticle = new List<Articles>();
            for (int i = Pages;i < 10; i++)
            {

                if (articles.Count > i)
                {
                    tenArticle.Add(articles[i]);
                }               
            }
            SearchResponse searchResponse = new SearchResponse();
            searchResponse.allPages = (double)Math.Ceiling((decimal)articles.Count / 10);
            searchResponse.Articles = tenArticle;
            return searchResponse;
        }
    }
}
