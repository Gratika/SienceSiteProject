namespace apiServer.Models
{
    public class People
    {
        public string Id { get; set; }
        public string? surname { get; set; }
        public string? name { get; set; }
        public DateTime? birthday { get; set; }
        public DateTime date_create { get; set; }
        public DateTime modified_date { get; set; }
        public string? path_bucket { get; set; }
    }
}
