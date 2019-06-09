using Newtonsoft.Json;
using System;
using System.Linq;

namespace FilmLister.TmdbIntegration.Models
{
    public class CastCrewContainer
    {
        [JsonProperty("cast")]
        public Cast[] Cast { get; set; }

        [JsonProperty("crew")]
        public Crew[] Crew { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets cast and crew as a unified person array. This is constructed as a union of those members.
        /// </summary>
        [JsonIgnore]
        public Person[] Persons
        {
            get
            {
                return Cast.Cast<Person>().Union(Crew).ToArray();
            }
        }
    }

    public class Cast : Person
    {
        public Cast()
        {
            Job = "Actor";
        }

        [JsonProperty("character")]
        public string Character { get; set; }

        [JsonIgnore]
        public override string Job { get => base.Job; set => base.Job = value; }
    }

    public class Crew : Person
    {
        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("job")]
        public override string Job { get; set; }
    }

    public abstract class Person
    {
        [JsonProperty("credit_id")]
        public string CreditId { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        [JsonProperty("vote_average")]
        public float VoteAverage { get; set; }

        [JsonProperty("video")]
        public bool Video { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("genre_ids")]
        public int?[] GenreIds { get; set; }

        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [JsonProperty("popularity")]
        public float Popularity { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

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

        public virtual string Job { get; set; }
    }
}
