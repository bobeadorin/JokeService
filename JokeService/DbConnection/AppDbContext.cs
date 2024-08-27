using Microsoft.EntityFrameworkCore;
using JokeService.Models.JokeModels;

namespace JokeService.DbConnection
{
    public class AppDbContext : DbContext
    {
        public IConfiguration _config { get; set; }

        public AppDbContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("DatabaseConnection"));
        }

        public DbSet<Joke> Jokes { get; set; }

    }
}
