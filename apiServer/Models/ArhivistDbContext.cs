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
            string mySqlConnectionString = $"server=mysql_db;port=3306;database=archivist;user=arch;password=g@o3LwoCtvHU_.SJ;";
            optionsBuilder.UseMySQL(mySqlConnectionString);
        }

    }
}
