namespace apiServer.Models
{
    public class Reactions
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public int article_id { get; set; }
        public int reaction_id { get; set; }
        public DateTime date_create { get; set; }
        public DateTime modified_date { get; set; }
    }
}
