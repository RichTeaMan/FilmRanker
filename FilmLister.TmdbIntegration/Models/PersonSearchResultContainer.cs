using Newtonsoft.Json;
using System;

namespace FilmLister.TmdbIntegration.Models
{
    public class PersonSearchResultContainer
    {
        public int page { get; set; }
        public int total_results { get; set; }
        public int total_pages { get; set; }
        public PersonSearchResult[] results { get; set; }
    }

    public class PersonSearchResult
    {
        public float popularity { get; set; }
        public int id { get; set; }
        public string profile_path { get; set; }
        public string name { get; set; }
        public PersonKnownFor[] known_for { get; set; }
        public bool adult { get; set; }
    }

    public class PersonKnownFor
    {
        public float vote_average { get; set; }
        public int vote_count { get; set; }
        public int id { get; set; }
        public bool video { get; set; }
        public string media_type { get; set; }
        public string title { get; set; }
        public float popularity { get; set; }
        public string poster_path { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public int[] genre_ids { get; set; }
        public string backdrop_path { get; set; }
        public bool adult { get; set; }
        public string overview { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDateString { get; set; }

        [JsonIgnore]
        public DateTimeOffset? ReleaseDate
        {
            get
            {
                DateTimeOffset? result = null;
                if (!string.IsNullOrWhiteSpace(ReleaseDateString))
                {
                    result = DateTimeOffset.Parse(ReleaseDateString);
                }
                return result;
            }
        }
    }

}
