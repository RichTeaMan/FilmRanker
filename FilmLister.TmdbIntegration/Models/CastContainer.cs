using Newtonsoft.Json;
using System;

namespace FilmLister.TmdbIntegration.Models
{
    public class CastCrewContainer
    {
        public Cast[] cast { get; set; }
        public Crew[] crew { get; set; }
        public int id { get; set; }
    }

    public class Cast
    {
        public string character { get; set; }
        public string credit_id { get; set; }
        public int vote_count { get; set; }
        public bool video { get; set; }
        public bool adult { get; set; }
        public float vote_average { get; set; }
        public string title { get; set; }
        public int?[] genre_ids { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public float popularity { get; set; }
        public int id { get; set; }
        public string backdrop_path { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }

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

    public class Crew
    {
        public int id { get; set; }
        public string department { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public string job { get; set; }
        public string overview { get; set; }
        public int vote_count { get; set; }
        public bool video { get; set; }
        public float vote_average { get; set; }
        public string title { get; set; }
        public float popularity { get; set; }
        public int?[] genre_ids { get; set; }
        public string backdrop_path { get; set; }
        public bool adult { get; set; }
        public string poster_path { get; set; }
        public string credit_id { get; set; }

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
