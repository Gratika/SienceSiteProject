namespace apiServer.Models
{
    public class Reactions
    {
        public int Id { get; set; }
        public string user_id { get; set; }
        public string article_id { get; set; }
        public string reaction_id { get; set; }
        public DateTime date_create { get; set; }
        public DateTime modified_date { get; set; }
    }
}
