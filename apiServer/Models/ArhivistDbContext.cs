using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace apiServer.Models
{
    public class ArhivistDbContext: DbContext
    {
        public DbSet<Emotions> Emotions { get; set; }
        public ArhivistDbContext(DbContextOptions<ArhivistDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            optionsBuilder.UseMySQL(connectionString);
        }

    }
}
