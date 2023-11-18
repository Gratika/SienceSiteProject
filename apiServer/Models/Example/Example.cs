using SolrNet.Attributes;
using System.Collections;

namespace apiServer.Models.Example
{
    public class Example
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("title")]
        public string title { get; set; }
        [SolrField("text")]
        public string text { get; set; }
        [SolrField("tag")]
        public string tag { get; set; }
        [SolrField("author")]
        public string author { get; set; }
        [SolrField("dataCreate")]
        public DateTime dataCreate { get; set; }
        [SolrField("views")]
        public int views { get; set; }
        [SolrField("DOI")]
        public string? DOI { get; set; }
    }
}
