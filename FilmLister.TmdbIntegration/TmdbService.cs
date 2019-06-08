using FilmLister.TmdbIntegration.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FilmLister.TmdbIntegration
{
    public class TmdbService : IDisposable
    {
        private readonly HttpClient client = new HttpClient();

        public string ApiKey { get; set; }

        public async Task<MovieDetails> FetchMovieDetails(int tmdbId)
        {
            string url = $"https://api.themoviedb.org/3/movie/{tmdbId}?api_key={ApiKey}";

            string json = await client.GetStringAsync(url);
            var movieDetails = JsonConvert.DeserializeObject<MovieDetails>(json);
            return movieDetails;
        }

        public async Task<MovieSearchResultContainer> SearchMovies(string query)
        {
            string url = $"https://api.themoviedb.org/3/search/movie?api_key={ApiKey}&language=en-US&query={query}";
            string json = await client.GetStringAsync(url);
            var movieSearchResult = JsonConvert.DeserializeObject<MovieSearchResultContainer>(json);
            return movieSearchResult;
        }

        public async Task<PersonSearchResultContainer> SearchPeople(string query)
        {
            string url = $"https://api.themoviedb.org/3/search/person?api_key={ApiKey}&language=en-US&query={query}";
            string json = await client.GetStringAsync(url);
            var personSearchResult = JsonConvert.DeserializeObject<PersonSearchResultContainer>(json);
            return personSearchResult;
        }

        public async Task<CastCrewContainer> FetchPersonMovieCredits(int tmdbPersonId)
        {
            string url = $"https://api.themoviedb.org/3/person/{tmdbPersonId}/movie_credits?api_key={ApiKey}&language=en-US";
            string json = await client.GetStringAsync(url);
            var castCrewResult = JsonConvert.DeserializeObject<CastCrewContainer>(json);
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
