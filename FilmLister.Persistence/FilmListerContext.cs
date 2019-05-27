using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace FilmLister.Persistence
{
    public class FilmListerContext : DbContext
    {
        public DbSet<Film> Films { get; set; }

        public DbSet<FilmListTemplate> FilmListTemplates { get; set; }

        public DbSet<FilmListItem> FilmListItem { get; set; }

        public DbSet<OrderedFilm> OrderedFilms { get; set; }

        public DbSet<OrderedList> OrderedLists { get; set; }

        public FilmListerContext(DbContextOptions<FilmListerContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();

            var connectionString = config.GetConnectionString("FilmListerDatabase");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
