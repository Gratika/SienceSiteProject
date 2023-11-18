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
        //public DbSet<User_roles> user_Roles { get; set; }
        public ArhivistDbContext(DbContextOptions<ArhivistDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            string connectionString = $"server=mysql_db;port=3306;database=archivist;user=root;password=root_password;";
            optionsBuilder.UseMySQL(connectionString);
        }

    }
}
