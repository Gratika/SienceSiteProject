using apiServer.Controllers.Redis;
using apiServer.Models.ForUser;
using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet;
using CommonServiceLocator;
using System.Reflection.Metadata;
using apiServer.Models.Example;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using System.Collections;
using SolrNet.Impl;
using Google.Protobuf.WellKnownTypes;
using StackExchange.Redis;
using System.Text;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        ISolrOperations<Example> solr;
       public SearchController()
       {
            // Инициализация <link>SolrNet</link>
            
        }
        [HttpPost("StartSolr")]
        public void StartSolr(/*int id*/) // возвращение конкретной статьи
        {
            
            
        }
        [HttpPost("AddArticle")]
        public ActionResult AddArticle(Articles article /*string title, string text, string tag, string author, int views, string? DOI*/) // возвращение конкретной статьи
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();
            //DateTime DataCreate = DateTime.Now;
            Example ex = new Example();
            ex.Id = article.Id;
            ex.title = article.title;
            ex.text = article.text;
            ex.tag = article.tag;
            ex.views = article.views;
            ex.author = article.author_id;
            ex.dataCreate = article.date_created;
            ex.DOI = article.DOI;

            solr.Add(ex);
            solr.Commit();

            return Ok();
        }
        [HttpPost("Search")]
        public ActionResult Search(string SearchString) // возвращение конкретной статьи
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

            var queryOptions = new QueryOptions
            {
                ExtraParams = new Dictionary<string, string>
                {
           { "defType", "edismax" },  // Используем расширенный запрос
           { "qf", "title text tag author" },           // Указываем поле для поиска
           { "mm", "5%" },           // Минимальное количество слов, которые должны совпадать
           { "pf", "title^2 text^1 tag^2 author^2" },          // Указываем вес полю Text
           { "spellcheck", "true" }, // Включение компонента автокоррекции
           { "spellcheck.dictionary", "default" }, // Использование словаря по умолчанию
           { "spellcheck.q", SearchString } // Передача текста для проверки
                }
            };

            var results = solr.Query(new SolrQuery(/*"task example~"*/SearchString + "~"), queryOptions);  // Укажите ваш запрос поиска

            string ResultForUser = "";
            // Обработка результатов
            foreach (var result in results)
            {
                ResultForUser += "Title: " + result.title + " Text: " + result.text + " Tag: " + result.tag + " Author: " + result.author + " DOI: " + "\n";
            }

            return Ok("Finally result - " + ResultForUser);
        }
        [HttpPost("DeleteDocumentsWithInvalidTitle")]
        public void DeleteDocumentsWithInvalidTitle(string Id)
        {
            ISolrOperations<Example> solr = ServiceLocator.Current.GetInstance<ISolrOperations<Example>>();

            solr.Delete(Id);
            solr.Commit();
        }
    }
}
