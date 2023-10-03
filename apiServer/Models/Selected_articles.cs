namespace apiServer.Models
{
    public class Selected_articles
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public int article_id { get; set; }
        public DateTime Date_view { get; set; }
    }
}
