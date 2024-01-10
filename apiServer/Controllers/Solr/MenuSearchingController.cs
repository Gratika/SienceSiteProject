using apiServer.Controllers.ForModels;
using apiServer.Controllers.Search;
using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace apiServer.Controllers.Solr
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuSearchingController : ControllerBase
    {
        private readonly SearchController _searchController;
        private readonly OrderingController _orderingController;
        private readonly FiltersController _filtersController;
        private readonly ReactionController _reactionController;
        private readonly string emojiId;
        public MenuSearchingController(SearchController searchController, OrderingController orderingController, FiltersController filtersController, ReactionController reactionController)
        {
            _searchController = searchController;
            _orderingController = orderingController;
            _filtersController = filtersController;
            _reactionController = reactionController;
            emojiId = "1";
        }

        [HttpGet("SearchWithFilters")]
        public ActionResult<List<Articles>> SearchWithFilters(string SearchString, int Pages,int? year, int? Filters, int? TypeOrder,string? tags, string? peopleId) // возвращение по наибольшему числу просмотров
        {
            try
            {
                //Filters = new List<int>();
                //Filters.Add(1);
                SearchResponse<Articles> searchResponse = SearchWithOrders(SearchString, Pages * 10, TypeOrder, tags, peopleId);
                
                    switch (Filters)
                    {
                        case 1:
                            searchResponse.Articles = _filtersController.OnlySciensceArticles(searchResponse.Articles);
                            break;
                        case 2:
                            searchResponse.Articles = _filtersController.SimpleArticles(searchResponse.Articles);
                            break;
                    }
              
                if (year != null)
                {
                    searchResponse.Articles = _filtersController.SelectYear(searchResponse.Articles, year);
                }

                SearchResponse<ArticleAndReactions> articlesAndReactions = new SearchResponse<ArticleAndReactions>();
                articlesAndReactions.Articles = new List<ArticleAndReactions>();
                articlesAndReactions.allPages = searchResponse.allPages;
                foreach (var article in searchResponse.Articles)
                {
                    ArticleAndReactions ar = _reactionController.GetReactionForArticle(article.Id, emojiId, article.author_id);
                    articlesAndReactions.Articles.Add(new ArticleAndReactions { Articles = article, Emotion = ar.Emotion, CountReactions = ar.CountReactions});
                }

            return Ok(articlesAndReactions);
        }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, не удалось найти статьи - " + ex.Message);
    }
}
        [HttpGet("SearchWithOrdersAndTags")]
        public SearchResponse<Articles> SearchWithOrders(string SearchString, int Pages, int? TypeOrder, string? tags,string? peopleId) // возвращение по наибольшему числу просмотров
        {
            try
            {
                List<Articles> articles = _searchController.Search(SearchString, tags, peopleId);
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
                for (int i = Pages; i < 10; i++)
                {

                    if (articles.Count > i)
                    {
                        tenArticle.Add(articles[i]);
                        
                    }
                }
                SearchResponse<Articles> searchResponse = new SearchResponse<Articles>();
                searchResponse.allPages = (double)Math.Ceiling((decimal)articles.Count / 10);
                searchResponse.Articles = articles;

                return searchResponse;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
