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
        ArhivistDbContext _context;
       
        public SolrArticleController(ArhivistDbContext context)
        {
            _context = context;
            ArticleSolr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
            PeopleSolr = ServiceLocator.Current.GetInstance<ISolrOperations<People>>();
            TheoriesSolr = ServiceLocator.Current.GetInstance<ISolrOperations<Scientific_theories>>();           
        }
        [HttpPost("AddArticle")]
        public ActionResult AddArticle<T>(T model /*string title, string text, string tag, string author, int views, string? DOI*/) // возвращение конкретной статьи
        {
            try
            {
                //DateTime DataCreate = DateTime.Now;
                //Articles ex = new Articles();
                //ex.Id = "dasdas-12312csa-cascsa2";
                //ex.title = "Example";
                //ex.text = "Example";
                //ex.tag = "Example";
                //ex.views = 14;
                ////ex.author_id = "1";
                //ex.date_created = DataCreate;
                //ex.modified_date = DataCreate;
                //ex.theory_id = "1";

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
                }
                

                return Ok(); // убрать потом слово
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddAllArticleFromDb")]
        public ActionResult AddAllArticleFromDb()
        {
            try
            {
                List<Articles> articles = _context.Articles.ToList();

                ArticleSolr.AddRange(articles);
                ArticleSolr.Commit();

                return Ok("Успешно данные добавлены в Solr");
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка, данные не добавлены" + ex.Message);
            }
        }
        [HttpPost("RedactArticle")]
        public void RedactArticle(Articles article)
        {

            //Example ex = new Example();
            //ex.Id = article.Id;
            //ex.title = article.title;
            //ex.text = article.text;
            //ex.tag = article.tag;
            //ex.views = article.views;
            //ex.author = article.author_id;
            //ex.dataCreate = article.date_created;
            //ex.DOI = article.DOI;
            try
            {
                ArticleSolr.Add(article, new AddParameters { Overwrite = true });
                ArticleSolr.Commit();
            }
            catch
            {
                throw new Exception();
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
            catch 
            {
                throw new Exception();
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
             catch 
            {
                throw new Exception();
            }
        }
        [HttpPost("Checking")]
        public ActionResult Checking()
        {
            DateTime DataCreate = DateTime.Now;
            Articles ex = new Articles();
            ex.Id = "dasdas-12312csa-cascsa2";
            ex.title = "Example";
            ex.text = "Example";
            ex.tag = "Example";
            ex.views = 14;
            ex.author_id = "7d35600a-c9e1-4826-99e7-abe7a23bd19d";
            ex.date_created = DataCreate;
            ex.modified_date = DataCreate;
            ex.theory_id = "bdde4549-5718-40e9-9859-72c080063958";

            //People ex = new People();
            //ex.Id = Guid.NewGuid().ToString();
            //ex.surname = "Example";
            //ex.name = "ExName";
            //ex.date_create = DateTime.Now;
            //ex.modified_date = DateTime.Now;

            //Scientific_theories ex = new Scientific_theories();
            //ex.Id = Guid.NewGuid().ToString();
            //ex.science_id = "1";
            //ex.note = "Example";
            //ex.name = "Test";

            return AddArticle(ex);
        }
        [HttpGet("GetArticle")]
        public List<Articles> GetArticle(string SearchStr, QueryOptions optionForSolr)
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
    }
}
