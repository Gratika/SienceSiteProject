namespace apiServer.Models
{
    public class Selected_articles
    {
        public string Id { get; set; }
        public string user_id { get; set; }
        public Users? user_ { get; set; }
        public string article_id { get; set; }
        public Articles? article_ { get; set; }
        public DateTime Date_view { get; set; }
    }
}
