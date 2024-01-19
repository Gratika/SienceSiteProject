using Microsoft.Extensions.Configuration.CommandLine;

namespace apiServer.Models.ForUser
{
    public class SearchResponse<T>
    {
        public List<T>? Articles { get; set; }
        public double allPages { get; set; }
        public Sciences? Sciences { get; set; }
        public List<Scientific_theories>? Theories { get; set; }
    }
}
