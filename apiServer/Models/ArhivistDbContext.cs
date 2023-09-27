using Microsoft.EntityFrameworkCore;

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
            string mySqlConnectionString = $"server=mysql_db;port=3306;database={Environment.GetEnvironmentVariable("MYSQL_DATABASE")};user={Environment.GetEnvironmentVariable("MYSQL_USER")};password={Environment.GetEnvironmentVariable("MYSQL_PASSWORD")};";
            optionsBuilder.UseMySQL(mySqlConnectionString);
        }

    }
}
