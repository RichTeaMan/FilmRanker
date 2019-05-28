using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FilmLister.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddUserSecrets("9b9374a9-4a72-4657-a398-2e265456aaf2");
                config.AddCommandLine(args);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            })
            .UseStartup<Startup>();
    }
}
