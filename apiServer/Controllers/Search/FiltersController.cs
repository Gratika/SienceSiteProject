using apiServer.Models.Example;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet.Commands.Parameters;
using SolrNet;
using CommonServiceLocator;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiltersController : ControllerBase
    {
        ISolrOperations<Example> solr;

        [HttpPost("OnlySciensceArticles")]
        public ActionResult OnlySciensceArticles(List<Example> articles) // возвращение статей от новых к старым
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

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
        public ActionResult SimpleArticles(List<Example> articles) // возвращение статей от новых к старым
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

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
