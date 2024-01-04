namespace apiServer.Models.ForUser
{
    public class SearchResponse<T>
    {
        public List<T>? Articles { get; set; }
        public double allPages { get; set; }
    }
}
