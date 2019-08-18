using FilmLister.Persistence;
using FilmLister.Service;
using FilmLister.TmdbIntegration;
using FilmLister.WebUI.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FilmLister.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            var connectionString = Configuration.GetConnectionString("FilmListerDatabase");

            string tmdbApiKey = Configuration.GetValue<string>("TmdbApiKey");

            BindConfig<AuthConfig>(services, "Auth");
            services.AddDbContext<FilmListerContext>(options => options.UseSqlServer(connectionString));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMemoryCache();
            services.AddTransient<FilmMapper>();
            services.AddTransient<FilmRankMapper>();
            services.AddTransient<FilmListMapper>();
            services.AddTransient(sp =>
            {
                return new TmdbServiceConfig
                {
                    ApiKey = tmdbApiKey
                };
            });
            services.AddSingleton<ChoicesRemainingService>(sp =>
            {
                var choicesRemaingService = new ChoicesRemainingService();
                try
                {
                    choicesRemaingService.LoadChoicesFromJson("choices.json").Wait();
                }
                catch(Exception ex)
                {
                    throw new Exception("Cannot build ChoicesRemainingService.", ex);
                }
                return choicesRemaingService;
            });
            services.AddTransient<TmdbService>();
            services.AddTransient<OrderService>();
            services.AddTransient<FilmService>();
            services.AddHostedService<FilmUpdateHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void BindConfig<T>(IServiceCollection serviceCollection, string configKey) where T : class, new()
        {
            var t = new T();
            Configuration.Bind(configKey, t);
            serviceCollection.AddSingleton(t);
        }
    }
}
