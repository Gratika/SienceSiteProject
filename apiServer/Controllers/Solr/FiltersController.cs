using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet.Commands.Parameters;
using SolrNet;
using CommonServiceLocator;
using apiServer.Models;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiltersController : ControllerBase
    {
        ISolrOperations<Articles> solr;

        public FiltersController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
        }
        [HttpPost("OnlySciensceArticles")]
        public ActionResult OnlySciensceArticles(List<Articles> articles) // возвращение статей от новых к старым
        {
            

            // ПРИМЕР                   
            // Поиск с учетом релевантности
            //var options = new QueryOptions
            //{
            //    Fields = new[] { "id", "title", "text", "tag", "author", "dataCreate", "views", "DOI", "score" }, // Возвращаемые поля
            //    OrderBy = new[] { new SortOrder("score", SolrNet.Order.DESC) } // Сортировка по релевантности
            //};

            //// Выполняем запрос
            //var results = solr.Query("*:*", options); // Ищем по строке "<link>example</link>"

            //articles = results;
            //ПРИМЕР

            var filteredModels = articles.Where(m => !string.IsNullOrEmpty(m.DOI)).ToArray();

            return Ok(filteredModels);
        }
        [HttpPost("SimpleArticles")]
        public ActionResult SimpleArticles(List<Articles> articles) // возвращение статей от новых к старым
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();

            // ПРИМЕР
            // Поиск с учетом релевантности
            //var options = new QueryOptions
            //{
            //    Fields = new[] { "id", "title", "text", "tag", "author", "dataCreate", "views", "DOI", "score" }, // Возвращаемые поля
            //    OrderBy = new[] { new SortOrder("score", SolrNet.Order.DESC) } // Сортировка по релевантности
            //};

            //// Выполняем запрос
            //var results = solr.Query("*:*", options); // Ищем по строке "<link>example</link>"

            //articles = results;
            //ПРИМЕР

            var filteredModels = articles.Where(m => string.IsNullOrEmpty(m.DOI)).ToArray();

            return Ok(filteredModels);
        }
    }
}
