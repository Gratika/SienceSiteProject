using apiServer.Controllers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace apiServer.Models
{
    public class ArhivistDbContext: DbContext
    {
        public DbSet<Emotions> Emotions { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Articles> Articles { get; set; }
        public DbSet<People> people { get; set; }
        public DbSet<Sciences> Sciences { get; set; }
        public DbSet<Scientific_theories> Scientific_theories { get; set; }
        public DbSet<Selected_articles> Selected_articles { get; set; }
        public DbSet<Reactions> Reactions { get; set; }      
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            string connectionString = $"server=mysql_db;port=3306;database=archivist;user=root;password=root_password;";
            optionsBuilder.UseMySQL(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
