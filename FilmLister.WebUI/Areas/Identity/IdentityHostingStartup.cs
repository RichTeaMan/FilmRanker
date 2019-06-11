using System;
using FilmLister.Persistence;
using FilmLister.WebUI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(FilmLister.WebUI.Areas.Identity.IdentityHostingStartup))]
namespace FilmLister.WebUI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<FilmListerContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("FilmListerDatabase")));

                services.AddDefaultIdentity<IdentityUser>()
                    .AddEntityFrameworkStores<FilmListerContext>();
            });
        }
    }
}