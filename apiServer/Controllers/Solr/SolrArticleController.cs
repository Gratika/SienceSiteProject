using apiServer.Models;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace apiServer.Controllers.Solr
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolrArticleController : ControllerBase
    {
        ISolrOperations<Articles> ArticleSolr;      
        ISolrOperations<People> PeopleSolr;
        ISolrOperations<Scientific_theories> TheoriesSolr;
        //ISolrOperations<Selected_articles> SelectedArticlesSolr;
        ArhivistDbContext _context;
       
        public SolrArticleController(ArhivistDbContext context)
        {
            _context = context;
            ArticleSolr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
            PeopleSolr = ServiceLocator.Current.GetInstance<ISolrOperations<People>>();
            TheoriesSolr = ServiceLocator.Current.GetInstance<ISolrOperations<Scientific_theories>>();
            //SelectedArticlesSolr = ServiceLocator.Current.GetInstance<ISolrOperations<Selected_articles>>();
        }
        [HttpPost("AddArticle")]
        public ActionResult AddArticle<T>(T model ) // возвращение конкретной статьи
        {
            try
            {              
                switch (model)
                {
                    case Articles article:
                        ArticleSolr.Add(article);
                        ArticleSolr.Commit();
                        break; 
                    case People people:
                        PeopleSolr.Add(people);
                        PeopleSolr.Commit();
                        break;
                    case Scientific_theories theoris:
                        TheoriesSolr.Add(theoris);
                        TheoriesSolr.Commit();
                        break;
                    //case Selected_articles selected:
                    //    SelectedArticlesSolr.Add(selected);
                    //    SelectedArticlesSolr.Commit();
                    //    break;
                }
                

                return Ok(); // убрать потом слово
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("AddAllFromDb")]
        public ActionResult AddAllArticleFromDb()
        {
            try
            {
                List<Articles> articles = _context.Articles.ToList();
                List<People> peoples = _context.people.ToList();
                List<Scientific_theories> theories = _context.Scientific_theories.ToList();
                List<Selected_articles> SelectedArticles = _context.Selected_articles.ToList();

                ArticleSolr.AddRange(articles);                         
                ArticleSolr.Commit();

                PeopleSolr.AddRange(peoples);
                PeopleSolr.Commit();

                TheoriesSolr.AddRange(theories);
                TheoriesSolr.Commit();

                //SelectedArticlesSolr.AddRange(SelectedArticles);
                //SelectedArticlesSolr.Commit();

                return Ok("Успешно данные добавлены в Solr");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("RedactArticle")]
        public void RedactArticle(Articles article)
        {
            try
            {
                ArticleSolr.Add(article, new AddParameters { Overwrite = true });
                ArticleSolr.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("DeleteArticle")]
        public void DeleteArticle(string Id)
        {
            try
            {
                ArticleSolr.Delete(Id);
                ArticleSolr.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("DeleteAllFromSolr")]
        public void DeleteAllFromSolr()
        {        
            try
            {
                ArticleSolr.Delete(SolrQuery.All);
                PeopleSolr.Delete(SolrQuery.All);
                TheoriesSolr.Delete(SolrQuery.All);
                ArticleSolr.Commit();
                PeopleSolr.Commit();
                TheoriesSolr.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("GetArticle")]
        public List<Articles> GetArticle(string SearchStr, QueryOptions optionForSolr)
        {
            try
            {
                var queryOptions = new QueryOptions
                {
                    ExtraParams = new Dictionary<string, string>
                    {
                      { "defType", "edismax" },  // Используем расширенный запрос
                      { "qf", "Id id" },           // Указываем поле для поиска                     
                    }
                };
                List<Articles> articles = ArticleSolr.Query(SearchStr, optionForSolr);
                foreach (var article in articles)
                {
                    var peopleData = PeopleSolr.Query(new SolrQuery(article.author_id), queryOptions).FirstOrDefault();
                    article.SetAuthor(peopleData);
                    var TheoriesData = TheoriesSolr.Query(new SolrQuery(article.theory_id), queryOptions).FirstOrDefault();
                    article.SetTheory(TheoriesData);
                }
                return articles;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
     }
}
