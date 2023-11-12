namespace apiServer.Models
{
    public class ArticleWithUserTokenModel
    {
        public int Id { get; set; }
        public string DOI { get; set; }
        public int author_id { get; set; }
        public string title { get; set; }
        public string tag { get; set; }
        public string text { get; set; }
        public int views { get; set; }
        public DateTime date_created { get; set; }
        public DateTime modified_date { get; set; }
        public int theory_id { get; set; }
        public string path_file { get; set; }
        public string login { get; set; }
        public string name { get; set; }
        public string firstname { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string email { get; set; }
        public int relevance { get; set; }
    }
}
