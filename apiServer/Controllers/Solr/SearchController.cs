using apiServer.Models;
using Microsoft.AspNetCore.Mvc;
using SolrNet;
using CommonServiceLocator;
using SolrNet.Commands.Parameters;
using apiServer.Controllers.Solr;
using Minio.DataModel.Tags;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        ISolrOperations<Articles> solr;
        SolrArticleController solrArticleController;
        public SearchController(ArhivistDbContext context, SolrArticleController solrArticleControllerNew)
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
            solrArticleController = solrArticleControllerNew;
        }
        [HttpGet("Search")]
        public List<Articles> Search(string SearchString, string? tags, string? peopleId) // возвращение конкретной статьи(страницы при выводе начинаются от 0)
        {
            try
            {
                var queryOptions = new QueryOptions
                {
                    ExtraParams = new Dictionary<string, string>
                {
           { "defType", "edismax" },  // Используем расширенный запрос
           { "qf", "title text tag author_id" },           // Указываем поле для поиска
           { "mm", "5%" },           // Минимальное количество слов, которые должны совпадать   
           { "pf", "title^2 text^1 tag^2 author_id^2" },          // Указываем вес полям
           { "spellcheck", "true" }, // Включение компонента автокоррекции
           { "spellcheck.dictionary", "default" }, // Использование словаря по умолчанию
           { "spellcheck.q", SearchString }, // Передача текста для проверки
                }
                };

                if (string.IsNullOrEmpty(tags) == false)
                {
                    var searchWords = tags.Split(',');
                    var tagFilter = string.Join(" AND ", searchWords.Select(word => $"tag:{word}"));

                    queryOptions.FilterQueries = new ISolrQuery[]
                    {
                   new SolrQuery(tagFilter)
                    };
                }
                if (string.IsNullOrEmpty(peopleId) == false)
                {
                    var filterQueries = queryOptions.FilterQueries.ToList();
                    filterQueries.Add(new SolrQuery($"author_id:\"{peopleId}\""));
                    queryOptions.FilterQueries = filterQueries.ToArray();
                }
                else
                {
                    var filterQueries = queryOptions.FilterQueries.ToList();
                    filterQueries.Add(new SolrQuery($"IsActive:\"{true}\""));
                    queryOptions.FilterQueries = filterQueries.ToArray();

                }

                List<Articles> articles = solrArticleController.GetArticle(SearchString + "~", queryOptions);
                return articles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //[HttpGet("SearchSelectedArtciles")]
        //public ActionResult SearchSelectedArtciles(string SearchString, string? peopleId)
        //{
        //    var queryOptions = new QueryOptions
        //    {
        //        ExtraParams = new Dictionary<string, string>
        //        {
        //   //{ "defType", "edismax" },  // Используем расширенный запрос
        //   //{ "qf", "title text tag author_id" },           // Указываем поле для поиска text tag author_id
        //   //{ "mm", "5%" },           // Минимальное количество слов, которые должны совпадать   
        //   //{ "pf", "title^2 text^1 tag^2 author_id^2" },          // Указываем вес полям
        //   //{ "spellcheck", "true" }, // Включение компонента автокоррекции
        //   //{ "spellcheck.dictionary", "default" }, // Использование словаря по умолчанию
        //   //{ "spellcheck.q", SearchString }, // Передача текста для проверки

        //   { "q", "*:*" },  // Запрос "все документы"
        //   //{ "fq", "{!join from=id to=article_id}id:*" }

        //   { "fq", $"people_id:\"{peopleId}\""}, // AND article_id:\"id\"

        //   //{ "fq", "article_id:\"id\"" }
        //   //{ "author_id", peopleId },
        //   //{ "article_id", "id" }
        //        }
        //    };
        //    //if (string.IsNullOrEmpty(peopleId) == false)
        //    //{
        //    //    var filterQueries = queryOptions.FilterQueries.ToList();
        //    //    filterQueries.Add(new SolrQuery($"author_id:{peopleId}"));
        //    //    queryOptions.FilterQueries = filterQueries.ToArray();
        //    //}

        //    var articles = solr.Query(SolrQuery.All, queryOptions);
        //    //
        //    //var articles = solr.Query($"title:\"{SearchString}\"");

        //    return Ok(articles);
        //}
    }
}