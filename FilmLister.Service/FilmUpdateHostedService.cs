using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FilmLister.Service
{
    public class FilmUpdateHostedService : IHostedService, IDisposable
    {
        private readonly ILogger logger;

        private Timer timer;

        public IServiceProvider Services { get; }

        public FilmUpdateHostedService(IServiceProvider services,
            ILogger<FilmUpdateHostedService> logger)
        {
            Services = services;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting update timer.");

            timer = new Timer((callback) =>
            {
                try
                {
                    var updateTask = UpdateFilmsAsync();
                    updateTask.Wait();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error running update task.");
                }
            },
            null,
            TimeSpan.FromMinutes(30),
            TimeSpan.FromMinutes(120));

            return Task.CompletedTask;
        }

        public async Task UpdateFilmsAsync()
        {
            logger.LogInformation("Updating films.");

            using (var scope = Services.CreateScope())
            {
                var filmService = scope.ServiceProvider.GetRequiredService<FilmService>();
                try
                {
                    await filmService.UpdateAllFilmsFromTmdb();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error updating films.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping update timer.");
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    timer?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
