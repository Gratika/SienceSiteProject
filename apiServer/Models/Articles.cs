using SolrNet.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public People? author_ {  get; private set; }
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
        public Scientific_theories? theory_ {  get; private set; }
        [SolrField("path_file")]
        public string? path_file { get; set; }
        [JsonIgnore]
        public string? urls { get; set; }
        [SolrField("IsActive")]
        public bool IsActive { get; set; }

        public void SetAuthor(People author)
        {
            author_ = author;
        }

        public void SetTheory(Scientific_theories theory)
        {
            theory_ = theory;
        }
    }
}
