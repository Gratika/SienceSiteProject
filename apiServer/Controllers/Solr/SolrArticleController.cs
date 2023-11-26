using apiServer.Models;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet;

namespace apiServer.Controllers.Solr
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolrArticleController : ControllerBase
    {
        ISolrOperations<Articles> solr;
        ArhivistDbContext _context;
        public SolrArticleController(ArhivistDbContext context)
        {
            _context = context;
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Articles>>();
        }
        [HttpPost("AddArticle")]
        public ActionResult AddArticle(/*Articles article*/ /*string title, string text, string tag, string author, int views, string? DOI*/) // возвращение конкретной статьи
        {
            
            DateTime DataCreate = DateTime.Now;
            Articles ex = new Articles();
            ex.Id = "dasdas-12312csa-cascsa2";
            ex.title = "Example";
            ex.text = "Example";
            ex.tag = "Example";
            ex.views = 14;
            ex.author_id = "1";
            ex.date_created = DataCreate;
            ex.modified_date = DataCreate;
            ex.theory_id = "1";

            solr.Add(ex);
            solr.Commit();

            return Ok();
        }
        [HttpPost("AddAllArticleFromDb")]
        public ActionResult AddAllArticleFromDb()
        {
            try
            {               
                List<Articles> articles = _context.Articles.ToList();

                solr.AddRange(articles);
                solr.Commit();

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

            solr.Add(article, new AddParameters { Overwrite = true });
            solr.Commit();
        }
        [HttpPost("DeleteArticle")]
        public void DeleteArticle(string Id)
        {
            solr.Delete(Id);
            solr.Commit();
        }
        [HttpPost("DeleteAllFromSolr")]
        public void DeleteAllFromSolr()
        {          
            solr.Delete(SolrQuery.All);
            solr.Commit();
        }
    }
}
