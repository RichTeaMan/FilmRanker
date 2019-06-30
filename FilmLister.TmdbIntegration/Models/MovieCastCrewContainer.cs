using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FilmLister.TmdbIntegration.Models
{
    public class MovieCastCrewContainer
    {
        [JsonProperty("cast")]
        public MovieCreditCast[] Cast { get; set; }

        [JsonProperty("crew")]
        public MovieCreditCrew[] Crew { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Fetches all crew that had a director job.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MovieCreditCrew> FetchDirectorJobs()
        {
            return Crew?.Where(c => c.Job == Constants.DIRECTOR_JOB_NAME) ?? new MovieCreditCrew[0];
        }

        public class MovieCreditPerson
        {
            [JsonProperty("cast_id")]
            public int CastId { get; set; }


            [JsonProperty("credit_id")]
            public string CreditId { get; set; }

            [JsonProperty("gender")]
            public int Gender { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("profile_path")]
            public string ProfilePath { get; set; }
        }

        public class MovieCreditCast : MovieCreditPerson
        {
            [JsonProperty("character")]
            public string Character { get; set; }

            [JsonProperty("order")]
            public int Order { get; set; }
        }

        public class MovieCreditCrew : MovieCreditPerson
        {
            [JsonProperty("department")]
            public string Department { get; set; }

            [JsonProperty("job")]
            public string Job { get; set; }

        }

    }
}
