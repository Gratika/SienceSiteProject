using apiServer.Controllers.Redis;
using apiServer.Models.ForUser;
using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet;
using CommonServiceLocator;
using System.Reflection.Metadata;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using System.Collections;
using SolrNet.Impl;
using Google.Protobuf.WellKnownTypes;
using StackExchange.Redis;
using System.Text;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        ISolrOperations<Articles> solr;
        ArhivistDbContext _context;
       public SearchController(ArhivistDbContext context)
       {
            _context = context;
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
        }              
        [HttpPost("Search")]
        public ActionResult Search(string SearchString) // возвращение конкретной статьи
        {          
            var queryOptions = new QueryOptions
            {
                ExtraParams = new Dictionary<string, string>
                {
           { "defType", "edismax" },  // Используем расширенный запрос
           { "qf", "title text tag author_id" },           // Указываем поле для поиска
           { "mm", "5%" },           // Минимальное количество слов, которые должны совпадать
           { "pf", "title^2 text^1 tag^2 author_id^2" },          // Указываем вес полю Text
           { "spellcheck", "true" }, // Включение компонента автокоррекции
           { "spellcheck.dictionary", "default" }, // Использование словаря по умолчанию
           { "spellcheck.q", SearchString } // Передача текста для проверки
                }
            };

            List<Articles> articles = solr.Query(new SolrQuery(/*"task example~"*/SearchString + "~"), queryOptions);  // Укажите ваш запрос поиска

            return Ok(articles);
        }        
    }
}
