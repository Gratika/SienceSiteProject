namespace apiServer.Models
{
    public class Users
    {
        public string Id { get; set; }
        public string? login { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public DateTime date_create { get; set; }
        public DateTime modified_date { get; set; }
        public string role_id { get; set; }
        public int email_is_checked { get; set; }
        public string people_id { get; set; }
        //public People People { get; set; }
    }
}
