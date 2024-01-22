using apiServer.Controllers.ForModels;
using apiServer.Controllers.Search;
using apiServer.Models;
using apiServer.Models.ForUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        private readonly ArhivistDbContext _context;
        private readonly string emojiId;
        public MenuSearchingController(SearchController searchController, OrderingController orderingController, FiltersController filtersController, ReactionController reactionController, ArhivistDbContext context)
        {
            _searchController = searchController;
            _orderingController = orderingController;
            _filtersController = filtersController;
            _reactionController = reactionController;
            _context = context;
            emojiId = "1";
        }

        [HttpGet("SearchWithFilters")]
        public async Task<ActionResult<List<Articles>>> SearchWithFilters( int Pages, string? SearchString, int? year, int? Filters, int? TypeOrder,string? tags, string? peopleId,string? scienceId, string? theoryId) // возвращение по наибольшему числу просмотров
        {
            try
            {
                //Filters = new List<int>();
                //Filters.Add(1);
                SearchResponse<Articles> searchResponse = await SearchWithOrders(Pages * 10, SearchString, TypeOrder, tags, peopleId);               

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
                if(!string.IsNullOrEmpty(scienceId))
                {
                searchResponse.Articles = searchResponse.Articles.Where(a => a.theory_.science_id == scienceId).ToList();
                }
            if (!string.IsNullOrEmpty(theoryId))
            {
                searchResponse.Articles = searchResponse.Articles.Where(a => a.theory_id == theoryId).ToList();
            }

            SearchResponse<FullArticle<Articles>> articlesAndReactions = new SearchResponse<FullArticle<Articles>>();
                articlesAndReactions.Articles = new List<FullArticle<Articles>>();
                articlesAndReactions.allPages = searchResponse.allPages;
            foreach (var article in searchResponse.Articles)
            {

                FullArticle<Articles> ar = await _reactionController.GetReactionForArticle<Articles>(article.Id, emojiId, article.author_id);
                ar.Selected = _context.Selected_articles.Any(a => a.article_id == article.Id && a.people_id == article.author_id);
                articlesAndReactions.Articles.Add(new FullArticle<Articles> { Articles = article, Emotion = ar.Emotion, CountReactions = ar.CountReactions, Selected = ar.Selected });
                if (!string.IsNullOrEmpty(scienceId))
                articlesAndReactions.Sciences = _context.Sciences.FirstOrDefault(a => a.Id == scienceId);
                articlesAndReactions.Theories = await _context.Scientific_theories.Where(t => t.science_id == scienceId).ToListAsync();
            }

            return Ok(articlesAndReactions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("SearchWithOrders")]
        public async Task<SearchResponse<Articles>> SearchWithOrders( int Pages, string? SearchString, int? TypeOrder, string? tags,string? peopleId) // возвращение по наибольшему числу просмотров
        {
            try
            {
                if (string.IsNullOrEmpty(SearchString))
                {
                    SearchString = "*";
                }
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
                for (int i = Pages; i < Pages + 10; i++)
                {
                    if (articles.Count > i)
                    {
                        tenArticle.Add(articles[i]);
                        
                    }
                }
                SearchResponse<Articles> searchResponse = new SearchResponse<Articles>();
                searchResponse.allPages = (double)Math.Ceiling((decimal)articles.Count / 10);
                searchResponse.Articles = tenArticle;

                return searchResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}   
