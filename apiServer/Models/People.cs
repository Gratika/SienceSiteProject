using SolrNet.Attributes;

namespace apiServer.Models
{
    public class People
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("surname")]
        public string? surname { get; set; }
        [SolrField("name")]
        public string? name { get; set; }
        [SolrField("birthday")]
        public DateTime? birthday { get; set; }
        [SolrField("date_create")]
        public DateTime date_create { get; set; }
        [SolrField("modified_date")]
        public DateTime modified_date { get; set; }
        [SolrField("path_bucket")]
        public string? path_bucket { get; set; }
    }
}
