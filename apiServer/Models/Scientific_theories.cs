using SolrNet.Attributes;

namespace apiServer.Models
{
    public class Scientific_theories
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("science_id")]
        public string science_id { get; set; }
        public Sciences science_ { get; set; }
        [SolrField("name")]
        public string? name { get; set; }
        [SolrField("note")]
        public string? note { get; set; }
    }
}
