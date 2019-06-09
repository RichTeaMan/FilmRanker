using Newtonsoft.Json;

namespace FilmLister.TmdbIntegration.Models
{
    public class ResultContainer<T>
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("results")]
        public T[] Results { get; set; }
    }
}
