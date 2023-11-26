using apiServer.Models;
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
        ISolrOperations<Articles> solr;

        public OrderingController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
        }
        [HttpPost("FromNewToOld")]
        public ActionResult FromNewToOld(List<Articles> articles) // возвращение статей от новых к старым
        {           
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

            articles.Sort((x, y) => y.date_created.CompareTo(x.date_created));
            return Ok(articles);
        }
        [HttpPost("FromOldToNew")]
        public ActionResult FromOldToNew(List<Articles> articles) // возвращение статей от старых к новым
        {
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

            articles.Sort((x, y) => x.date_created.CompareTo(y.date_created));
            return Ok(articles);
        }
        [HttpPost("ForViews")]
        public ActionResult ForViews(List<Articles> articles) // возвращение по наибольшему числу просмотров
        {
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
