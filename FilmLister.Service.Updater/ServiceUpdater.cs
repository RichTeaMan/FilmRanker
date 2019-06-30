using FilmLister.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.Service.Updater
{
    public class ServiceUpdater
    {
        private readonly ILogger logger;

        private readonly IServiceProvider serviceProvider;

        public ServiceUpdater(ILogger<ServiceUpdater> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task RunUpdateAsync()
        {
            logger.LogInformation("Starting update.");

            using (var scope = serviceProvider.CreateScope())
            {
                var filmUpdateHostedService = scope.ServiceProvider.GetRequiredService<FilmUpdateHostedService>();
                await filmUpdateHostedService.UpdateFilmsAsync();
            }

            // used for rebuilding lists if the completed state has changed.
            using (var scope = serviceProvider.CreateScope())
            {
                var filmUpdateHostedService = scope.ServiceProvider.GetRequiredService<FilmService>();
                var context = scope.ServiceProvider.GetRequiredService<FilmListerContext>();

                var uncompletedLists = context.OrderedLists.Where(l => l.Completed && !l.CompletedDateTime.HasValue).ToArray();
                foreach(var uncompletedList in uncompletedLists)
                {
                    uncompletedList.Completed = false;
                    await filmUpdateHostedService.AttemptListOrder(uncompletedList.Id);
                }
            }
        }
    }
}
