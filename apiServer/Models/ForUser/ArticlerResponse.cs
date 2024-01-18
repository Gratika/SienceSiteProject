namespace apiServer.Models.ForUser
{
    public class ArticlerResponse
    {
        public List<FullArticle<Articles>> Articles { get; set; }
        public string Response { get; set; }
    }
}
