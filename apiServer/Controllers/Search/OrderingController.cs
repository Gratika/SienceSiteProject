using apiServer.Models.Example;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderingController : ControllerBase
    {
        ISolrOperations<Example> solr;

        [HttpPost("FromNewToOld")]
        public ActionResult FromNewToOld(List<Example> articles) // возвращение статей от новых к старым
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

            // ПРИМЕР
            // Поиск с учетом релевантности
            //var options = new QueryOptions
            //{
            //    Fields = new[] { "id", "title", "text", "tag", "author", "dataCreate", "views", "score" }, // Возвращаемые поля
            //    OrderBy = new[] { new SortOrder("score", SolrNet.Order.DESC) } // Сортировка по релевантности
            //};

            //// Выполняем запрос
            //var results = solr.Query("*:*", options); // Ищем по строке "<link>example</link>"

            //articles = results;
            //ПРИМЕР

            articles.Sort((x, y) => y.dataCreate.CompareTo(x.dataCreate));
            return Ok(articles);
        }
        [HttpPost("FromOldToNew")]
        public ActionResult FromOldToNew(List<Example> articles) // возвращение статей от старых к новым
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

            // ПРИМЕР
            // Поиск с учетом релевантности
            //var options = new QueryOptions
            //{
            //    Fields = new[] { "id", "title", "text", "tag", "author", "dataCreate", "views", "score" }, // Возвращаемые поля
            //    OrderBy = new[] { new SortOrder("score", SolrNet.Order.DESC) } // Сортировка по релевантности
            //};

            //// Выполняем запрос
            //var results = solr.Query("*:*", options); // Ищем по строке "<link>example</link>"

            //articles = results;
            //ПРИМЕР

            articles.Sort((x, y) => x.dataCreate.CompareTo(y.dataCreate));
            return Ok(articles);
        }
        [HttpPost("ForViews")]
        public ActionResult ForViews(List<Example> articles) // возвращение по наибольшему числу просмотров
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

            // ПРИМЕР
            // Поиск с учетом релевантности
            //var options = new QueryOptions
            //{
            //    Fields = new[] { "id", "title", "text", "tag", "author", "dataCreate", "views", "score" }, // Возвращаемые поля
            //    OrderBy = new[] { new SortOrder("score", SolrNet.Order.DESC) } // Сортировка по релевантности
            //};

            //// Выполняем запрос
            //var results = solr.Query("*:*", options); // Ищем по строке "<link>example</link>"

            //articles = results;
            //ПРИМЕР

            articles.Sort((x, y) => y.views.CompareTo(x.views));
            return Ok(articles);
        }
    }
}
