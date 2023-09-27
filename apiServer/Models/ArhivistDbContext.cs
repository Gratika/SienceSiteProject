using Microsoft.EntityFrameworkCore;

namespace apiServer.Models
{
    public class ArhivistDbContext: DbContext
    {
        public DbSet<Emotions> Emotions { get; set; }
        public ArhivistDbContext(DbContextOptions<ArhivistDbContext> options) : base(options)
        {
        }

    }
}
