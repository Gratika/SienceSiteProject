namespace apiServer.Models
{
    public class Scientific_theories
    {
        public int Id { get; set; }
        public int science_id { get; set; }
        public string name { get; set; }
        public string? note { get; set; }
    }
}
