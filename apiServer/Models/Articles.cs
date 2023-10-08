﻿namespace apiServer.Models
{
    public class Articles
    {
        public int Id { get; set; }
        public string? DOI { get; set; }
        public int author_id { get; set; }
        public string title { get; set; }
        public string tag { get; set; }
        public string? text { get; set; }
        public int views { get; set; }
        public DateTime date_created { get; set; }
        public DateTime modified_date { get; set; }
        public int theory_id { get; set; }
        public string? path_file { get; set; }

    }
}