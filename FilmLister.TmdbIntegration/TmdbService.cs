using FilmLister.TmdbIntegration.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FilmLister.TmdbIntegration
{
    public class TmdbService : IDisposable
    {
        private readonly ILogger logger;

        private readonly HttpClient client = new HttpClient();

        public TmdbServiceConfig TmdbServiceConfig { get; }

        public TmdbService(ILogger<TmdbService> logger, TmdbServiceConfig tmdbServiceConfig)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            TmdbServiceConfig = tmdbServiceConfig ?? throw new ArgumentNullException(nameof(tmdbServiceConfig));
        }

        private async Task<string> FetchResponse(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && response.Headers.RetryAfter != null)
            {
                DateTimeOffset retryDate = DateTimeOffset.Now;
                if (response.Headers.RetryAfter.Date.HasValue)
                {
                    retryDate = response.Headers.RetryAfter.Date.Value;
                }
                else if (response.Headers.RetryAfter.Delta.HasValue)
                {
                    retryDate = DateTimeOffset.Now.Add(response.Headers.RetryAfter.Delta.Value);
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }
                logger.LogWarning($"Too many requests. Waiting till {retryDate}.");
                while (DateTimeOffset.Now < retryDate)
                {
                    await Task.Delay(500);
                }
                return await FetchResponse(url);
            }
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        public async Task<MovieDetails> FetchMovieDetails(int tmdbId)
        {
            string url = $"https://api.themoviedb.org/3/movie/{tmdbId}?api_key={TmdbServiceConfig.ApiKey}";

            string json = await FetchResponse(url);
            var movieDetails = JsonConvert.DeserializeObject<MovieDetails>(json);
            return movieDetails;
        }

        public async Task<ResultContainer<MovieSearchResult>> SearchMovies(string query)
        {
            string url = $"https://api.themoviedb.org/3/search/movie?api_key={TmdbServiceConfig.ApiKey}&language=en-US&query={query}";
            string json = await FetchResponse(url);
            var movieSearchResult = JsonConvert.DeserializeObject<ResultContainer<MovieSearchResult>>(json);
            return movieSearchResult;
        }

        public async Task<ResultContainer<PersonSearchResult>> SearchPeople(string query)
        {
            string url = $"https://api.themoviedb.org/3/search/person?api_key={TmdbServiceConfig.ApiKey}&language=en-US&query={query}";
            string json = await FetchResponse(url);
            var personSearchResult = JsonConvert.DeserializeObject<ResultContainer<PersonSearchResult>>(json);
            return personSearchResult;
        }

        public async Task<CastCrewContainer> FetchPersonMovieCredits(int tmdbPersonId)
        {
            string url = $"https://api.themoviedb.org/3/person/{tmdbPersonId}/movie_credits?api_key={TmdbServiceConfig.ApiKey}&language=en-US";
            string json = await FetchResponse(url);
            var castCrewResult = JsonConvert.DeserializeObject<CastCrewContainer>(json);
            return castCrewResult;
        }

        public async Task<MovieCastCrewContainer> FetchMovieCredits(int tmdbMovieId)
        {
            string url = $"https://api.themoviedb.org/3/movie/{tmdbMovieId}/credits?api_key={TmdbServiceConfig.ApiKey}&language=en-US";
            string json = await FetchResponse(url);
            var castCrewResult = JsonConvert.DeserializeObject<MovieCastCrewContainer>(json);
            return castCrewResult;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
