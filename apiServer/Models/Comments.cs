namespace apiServer.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public int parent_id { get; set; }
        public int user_id { get; set; }
        public int article_id { get; set; }
        public string text { get; set; }
        public DateTime date_created { get; set; }
        public DateTime modified_date { get; set; }
    }
}
