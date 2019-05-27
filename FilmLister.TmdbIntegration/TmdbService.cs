using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
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

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

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
