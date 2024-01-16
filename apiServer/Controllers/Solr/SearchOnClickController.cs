using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolrNet.Commands.Parameters;
using SolrNet;
using CommonServiceLocator;
using apiServer.Models;
using apiServer.Controllers.Solr;

namespace apiServer.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchOnClickController : ControllerBase
    {
        SolrArticleController solrArticleController;

        public SearchOnClickController( SolrArticleController solrArticleControllerNew)
        {
            solrArticleController = solrArticleControllerNew;
        }


    }
}
