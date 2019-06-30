using FilmLister.Persistence;
using FilmLister.TmdbIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FilmLister.Service.Updater
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("FilmLister Service Updater.");

            var serviceProvider = CreateServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var serviceUpdater = scope.ServiceProvider.GetRequiredService<ServiceUpdater>();
                await serviceUpdater.RunUpdateAsync();
            }
        }

        public static IServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddUserSecrets("9b9374a9-4a72-4657-a398-2e265456aaf2");

            var config = builder.Build();

            string tmdbApiKey = config.GetValue<string>("TmdbApiKey");
            var connectionString = config.GetConnectionString("FilmListerDatabase");

            serviceCollection.AddLogging(logging =>
             {
                 logging.AddConsole();
             });

            serviceCollection.AddDbContext<FilmListerContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient(sp =>
            {
                return new TmdbService
                {
                    ApiKey = tmdbApiKey
                };
            });
            serviceCollection.AddTransient<OrderService>();
            serviceCollection.AddTransient<FilmService>();
            serviceCollection.AddTransient<ServiceUpdater>();
            serviceCollection.AddTransient<FilmUpdateHostedService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
