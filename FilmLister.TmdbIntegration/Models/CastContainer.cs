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
        public string character { get; set; }
    }

    public class Crew : Person
    {
        public string department { get; set; }
        public string job { get; set; }
    }

    public class Person
    {
        public string credit_id { get; set; }
        public int vote_count { get; set; }
        public float vote_average { get; set; }
        public bool video { get; set; }
        public bool adult { get; set; }
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
}
