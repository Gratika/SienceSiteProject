namespace apiServer.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string? firstname { get; set; }
        public string? name { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public DateTime? birthday { get; set; }
        public DateTime date_create { get; set; }
        public DateTime modified_date { get; set; }
        public int role_id { get; set; }
    }
}
