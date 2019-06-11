using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FilmLister.Persistence
{
    public class FilmListerContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Film> Films { get; set; }

        public DbSet<FilmListTemplate> FilmListTemplates { get; set; }

        public DbSet<FilmListItem> FilmListItems { get; set; }

        public DbSet<OrderedFilm> OrderedFilms { get; set; }

        public DbSet<OrderedList> OrderedLists { get; set; }

        public DbSet<OrderedFilmRankItem> OrderedFilmRankItems { get; set; }

        public FilmListerContext(DbContextOptions<FilmListerContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            var config = builder.Build();

            var connectionString = config.GetConnectionString("FilmListerDatabase");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
