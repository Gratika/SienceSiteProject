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
        public ActionResult FromNewToOld(List<Example> articles) // возвращение конкретной статьи
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

            // Поиск с учетом релевантности
            var options = new QueryOptions
            {
                Fields = new[] { "id", "title", "text", "tag", "author", "dataCreate", "views", "score" }, // Возвращаемые поля
                OrderBy = new[] { new SortOrder("score", SolrNet.Order.DESC) } // Сортировка по релевантности
            };

            // Выполняем запрос
            var results = solr.Query("*:*", options); // Ищем по строке "<link>example</link>"

            //string ResultForUser = "";
            //// Обработка результатов
            //foreach (var article in articles)
            //{
            //    foreach (var result in results)
            //    {
            //        //ResultForUser += "Title: " + result.title + "\n";
            //        articles = result;
            //    }
            //}

            articles = results;

            articles.Sort((x, y) => y.dataCreate.CompareTo(x.dataCreate));
            return Ok(articles);
        }
    }
}
