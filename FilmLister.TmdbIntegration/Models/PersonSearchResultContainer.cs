using Newtonsoft.Json;
using System;

namespace FilmLister.TmdbIntegration.Models
{
    public class PersonSearchResult
    {
        [JsonProperty("popularity")]
        public float Popularity { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("known_for")]
        public PersonKnownFor[] KnownFor { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }
    }

    public class PersonKnownFor
    {
        [JsonProperty("vote_average")]
        public float VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("video")]
        public bool Video { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("popularity")]
        public float Popularity { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [JsonProperty("genre_ids")]
        public int[] GenreIds { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

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
