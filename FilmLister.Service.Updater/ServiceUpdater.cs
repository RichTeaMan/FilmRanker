using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FilmLister.Service.Updater
{
    public class ServiceUpdater
    {
        private readonly ILogger logger;

        private readonly FilmUpdateHostedService filmUpdateHostedService;

        public ServiceUpdater(ILogger<ServiceUpdater> logger, FilmUpdateHostedService filmUpdateHostedService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.filmUpdateHostedService = filmUpdateHostedService ?? throw new ArgumentNullException(nameof(filmUpdateHostedService));
        }

        public async Task RunUpdateAsync()
        {
            logger.LogInformation("Starting update.");
            await filmUpdateHostedService.UpdateFilmsAsync();
        }
    }
}
