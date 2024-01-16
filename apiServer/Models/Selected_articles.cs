using SolrNet.Attributes;

namespace apiServer.Models
{
    public class Selected_articles
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("people_id")]
        public string people_id { get; set; }
        public People? people_ { get; set; }
        [SolrField("article_id")]
        public string article_id { get; set; }
        public Articles? article_ { get; set; }
        [SolrField("Date_view")]
        public DateTime Date_view { get; set; }
        public void SetAuthor(People people)
        {
            people_ = people;
        }

        public void SetTheory(Articles articles)
        {
            article_ = articles;
        }
    }
}
