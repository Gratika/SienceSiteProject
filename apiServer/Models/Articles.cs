using SolrNet.Attributes;

namespace apiServer.Models
{
    public class Articles
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("DOI")]
        public string? DOI { get; set; }
        [SolrField("author_id")]
        public string author_id { get; set; }
        [SolrField("title")]
        public string title { get; set; }
        [SolrField("tag")]
        public string tag { get; set; }
        [SolrField("text")]
        public string? text { get; set; }
        [SolrField("views")]
        public int views { get; set; }
        [SolrField("date_created")]
        public DateTime date_created { get; set; }
        [SolrField("modified_date")]
        public DateTime modified_date { get; set; }
        [SolrField("theory_id")]
        public string theory_id { get; set; }
        [SolrField("path_file")]
        public string? path_file { get; set; }

    }
}
