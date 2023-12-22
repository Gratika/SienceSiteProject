using apiServer.Controllers.Search;
using apiServer.Models;
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
        public ActionResult<List<Articles>> SearchWithFilters(string SearchString, int pages, List<int> Filters) // возвращение по наибольшему числу просмотров
        {
            try
            {
                Filters = new List<int>();
                Filters.Add(0);
                Filters.Add(4);
                List<Articles> articles = _searchController.Search(SearchString, pages);
                for(int i = 0;i < Filters.Count; i++)
                {
                    switch (Filters[i]) 
                    {
                        case 0:
                            articles = _filtersController.OnlySciensceArticles(articles);
                            break;                           
                        case 1:
                            _filtersController.SimpleArticles(articles);
                            break; 
                        case 2:
                            articles = _orderingController.FromNewToOld(articles);
                            break;
                        case 3:
                            _orderingController.FromOldToNew(articles);
                            break;
                        case 4:
                            _orderingController.ForViews(articles);
                            break;
                    }
                }

                return Ok(articles);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
            }
        }
    }
}
